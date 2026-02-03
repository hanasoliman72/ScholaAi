using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories;
using ScholaAi.Repositories.Base;

public class requestBroadcastRepository : genericRepository<requestBroadcast>, IRequestBroadcastRepository
{
    private readonly DBcontext _context;

    public requestBroadcastRepository(DBcontext context) : base(context)
    {
        _context = context;
    }

    public async Task Add(requestBroadcast b)
    {
        await _context.requestBroadcasts.AddAsync(b); 
    }

    public async Task Accept(int teacherId, int sessionId)
    {
        var broadcast = await _context.requestBroadcasts
            .FirstOrDefaultAsync(b => b.teacherId == teacherId && b.requestId == sessionId);

        if (broadcast != null)
            broadcast.isAccepted = true;
    }

    public async Task Remove(int teacherId, int sessionId)
    {
        var broadcast = await _context.requestBroadcasts
            .FirstOrDefaultAsync(b => b.teacherId == teacherId && b.requestId == sessionId);

        if (broadcast != null)
            _context.requestBroadcasts.Remove(broadcast);
    }

    public async Task RemoveOthers(int sessionId, int teacherId)
    {
        var others = await _context.requestBroadcasts
            .Where(b => b.requestId == sessionId && b.teacherId != teacherId)
            .ToListAsync();

        _context.requestBroadcasts.RemoveRange(others);
    }

    public async Task<List<teacherRequestDto>> GetForTeacher(int teacherId)
    {
        return await _context.requestBroadcasts
            .Where(b => b.teacherId == teacherId && b.isAccepted == false)
            .Include(b => b.teacherSession)
                .ThenInclude(r => r.student)
                    .ThenInclude(s => s.user)
            .Include(b => b.teacherSession)
                .ThenInclude(r => r.subject)
            .Select(b => new teacherRequestDto
            {
                sessionId = b.requestId,
                studentName = b.teacherSession.student.user.firstName + " " +
                              b.teacherSession.student.user.lastName,
                subject = b.teacherSession.subject.name,
                preferredDate = b.teacherSession.preferredDate,
                description = b.teacherSession.description
            })
            .ToListAsync();
    }
}
