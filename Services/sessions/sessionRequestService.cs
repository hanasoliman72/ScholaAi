using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

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
    public async Task CreateRequest(int studentId, createSessionRequestDto dto)
    {
        var request = new sessionRequest
        {
            studentId = studentId,
            subjectId = dto.subjectId,
            preferredDate = dto.preferredDate,
            description = dto.description,
            status = requestStatus.Pending
        };

        await _requestRepo.Add(request);
        await _requestRepo.Save();

        var teachers = await _context.teachers
         .Where(t => t.subjectId == dto.subjectId)
         .Select(t => t.userId)
         .ToListAsync();


        foreach (var teacherId in teachers)
        {
            _context.requestBroadcasts.Add(new requestBroadcast
            {
                teacherId = teacherId,
                requestId = request.requestId
            });
        }

        await _context.SaveChangesAsync();
    }
    public async Task<List<teacherRequestDto>> GetTeacherRequests(int teacherId)
    {
        return await _broadcastRepo.GetForTeacher(teacherId);
    }

    public async Task AcceptRequest(int teacherId, int sessionId)
    {
        var request = await _requestRepo.GetById(sessionId);

        if (request == null)
            throw new Exception("Request not found");

        if (request.status != requestStatus.Pending)
            throw new Exception("Already taken");

        request.teacherId = teacherId;
        request.status = requestStatus.Accepted;
        request.finalScheduledAt = DateTime.Now;

        await _broadcastRepo.Accept(teacherId, sessionId);
        await _broadcastRepo.RemoveOthers(sessionId, teacherId);

        await _requestRepo.Save();
    }

    public async Task RejectRequest(int teacherId, int sessionId)
    {
        await _broadcastRepo.Remove(teacherId, sessionId);
        await _requestRepo.Save();
    }
    public async Task<List<studentSessionDto>> GetStudentRequests(int studentId)
    {
        var requests = await _requestRepo.GetForStudent(studentId);

        return requests.Select(r => new studentSessionDto
        {
            sessionId = r.requestId,
            subject = r.subject.name,
            preferredDate = r.preferredDate,
            status = r.status.ToString(),
            teacherName = r.teacher == null ? null :
                r.teacher.user.firstName + " " + r.teacher.user.lastName
        }).ToList();
    }
}
