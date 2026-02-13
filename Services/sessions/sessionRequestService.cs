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
        private readonly DBcontext _context;

        public sessionRequestService(
            ISessionRequestRepository requestRepo,
            IRequestBroadcastRepository broadcastRepo,
            DBcontext context)
        {
            _requestRepo = requestRepo;
            _broadcastRepo = broadcastRepo;
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

            foreach (var teacherId in teachers)
            {
                _context.RequestBroadcasts.Add(new RequestBroadcast
                {
                    TeacherId = teacherId,
                    RequestId = request.RequestId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<teacherRequestDto>> GetTeacherRequests(string teacherId)
        {
            return await _broadcastRepo.GetForTeacher(teacherId);
        }

        public async Task AcceptRequest(string teacherId, int sessionId)
        {
            var request = await _requestRepo.GetById(sessionId);

            if (request == null)
                throw new Exception("Request not found");

            if (request.Status != RequestStatus.Pending)
                throw new Exception("Already taken");

            request.TeacherId = teacherId;
            request.Status = RequestStatus.Accepted;
            request.FinalScheduledAt = DateTime.UtcNow;

            await _broadcastRepo.Accept(teacherId, sessionId);
            await _broadcastRepo.RemoveOthers(sessionId, teacherId);

            await _requestRepo.Save();
        }

        public async Task RejectRequest(string teacherId, int sessionId)
        {
            await _broadcastRepo.Remove(teacherId, sessionId);
            await _requestRepo.Save();
        }

        public async Task<List<studentSessionDto>> GetStudentRequests(string studentId)
        {
            var requests = await _requestRepo.GetForStudent(studentId);

            return requests.Select(r => new studentSessionDto
            {
                sessionId = r.RequestId,
                subject = r.Subject.name,
                preferredDate = r.PreferredDate,
                status = r.Status.ToString(),
                teacherName = r.Teacher == null
                    ? null
                    : $"{r.Teacher.FirstName} {r.Teacher.LastName}"
            }).ToList();
        }
    }
}
