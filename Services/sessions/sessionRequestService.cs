using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.sessions
{
    public class sessionRequestService : ISessionRequestService
    {
        private readonly ISessionRequestRepository _requestRepo;
        private readonly IRequestBroadcastRepository _broadcastRepo;
        private readonly INotificationService _notificationService;
        private readonly DBcontext _context;

        public sessionRequestService(
            ISessionRequestRepository requestRepo,
            IRequestBroadcastRepository broadcastRepo,INotificationService notificationService,
            DBcontext context)
        {
            _requestRepo = requestRepo;
            _broadcastRepo = broadcastRepo;
            _notificationService = notificationService;
            _context = context;

        }

        public async Task CreateRequest(string studentId, createSessionRequestDto dto)
        {
            var request = new SessionRequest
            {
                StudentId = studentId,
                SubjectId = dto.subjectId,
                PreferredDate = dto.preferredDate,
                Description = dto.description,
                Status = RequestStatus.Pending
            };

            await _requestRepo.Add(request);
            await _requestRepo.Save();

            // Get all teachers that teach the requested subject
            var teachers = await _context.Teachers
                .Where(t => t.SubjectId == dto.subjectId)
                .Select(t => t.ApplicationUserId)
                .ToListAsync();

            // Add broadcasts for all matching teachers
            foreach (var teacherId in teachers)
            {
                await _broadcastRepo.Add(new RequestBroadcast
                {
                    TeacherId = teacherId,
                    RequestId = request.RequestId
                });
               
                await _notificationService.SendNotification(
                    studentId,
                    teacherId,
                    "New session request available",
                    NotificationType.Request,
                    null,
                    request.RequestId
                );
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<teacherRequestDto>> GetTeacherRequests(string teacherId)
        {
            return await _broadcastRepo.GetForTeacher(teacherId);
        }

        public async Task AcceptRequest(string teacherId, int sessionId)
        {
            // ? Use transaction to prevent race conditions
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var request = await _requestRepo.GetById(sessionId);

                if (request == null)
                    throw new Exception("Request not found");

                // ? Check if teacher is authorized to accept this request
                var broadcast = await _context.RequestBroadcasts
                    .FirstOrDefaultAsync(b => b.TeacherId == teacherId && b.RequestId == sessionId);

                if (broadcast == null)
                    throw new Exception("You are not authorized to accept this request");

                // ? Double-check status (race condition protection)
                if (request.Status != RequestStatus.Pending)
                    throw new Exception("Request already accepted by another teacher");

                // Update request
                request.TeacherId = teacherId;
                request.Status = RequestStatus.Accepted;
                request.FinalScheduledAt = DateTime.UtcNow;

                // Update broadcast
                await _broadcastRepo.Accept(teacherId, sessionId);


                // Remove other teachers' broadcasts
                await _broadcastRepo.RemoveOthers(sessionId, teacherId);

                await _requestRepo.Save();
                await transaction.CommitAsync();
                await _notificationService.SendNotification(
                    teacherId,
                    request.StudentId,
                    "Your session request has been accepted",
                    NotificationType.Session,
                    null,
                    request.RequestId
                );
                //await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RejectRequest(string teacherId, int sessionId)
        {
            // ? Check if teacher has this broadcast
            var broadcast = await _context.RequestBroadcasts
                .FirstOrDefaultAsync(b => b.TeacherId == teacherId && b.RequestId == sessionId);

            if (broadcast == null)
                throw new Exception("Request not found or already removed");

            await _broadcastRepo.Remove(teacherId, sessionId);
            await _context.SaveChangesAsync(); // ? FIX: Was missing!
        }

        public async Task<List<studentRequestDto>> GetStudentRequests(string studentId)
        {
            var requests = await _requestRepo.GetForStudent(studentId);

            return requests.Select(r => new studentRequestDto
            {
                sessionId = r.RequestId,
                subject = r.Subject.name,
                preferredDate = r.PreferredDate,
                status = r.Status.ToString(),
                teacherName = r.Teacher?.ApplicationUser == null
                    ? null
                    : $"{r.Teacher.ApplicationUser.FirstName} {r.Teacher.ApplicationUser.LastName}"
            }).ToList();
        }
    }
}
