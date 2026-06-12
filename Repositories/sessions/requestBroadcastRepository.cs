using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;
using ScholaAi.Repositories;
using ScholaAi.Repositories.Base;

public class requestBroadcastRepository : genericRepository<RequestBroadcast>, IRequestBroadcastRepository
{
    private readonly DBcontext _context;

    public requestBroadcastRepository(DBcontext context) : base(context)
    {
        _context = context;
    }

    public async Task Add(RequestBroadcast b)
    {
        await _context.RequestBroadcasts.AddAsync(b); 
    }

    public async Task Accept(string teacherId, int sessionId)
    {
        var broadcast = await _context.RequestBroadcasts
            .FirstOrDefaultAsync(b => b.TeacherId == teacherId && b.RequestId == sessionId);

        if (broadcast != null)
            broadcast.IsAccepted = true;
    }

    public async Task Remove(string teacherId, int sessionId)
    {
        var broadcast = await _context.RequestBroadcasts
            .FirstOrDefaultAsync(b => b.TeacherId == teacherId && b.RequestId == sessionId);

        if (broadcast != null)
            _context.RequestBroadcasts.Remove(broadcast);
    }

    public async Task RemoveOthers(int sessionId, string teacherId)
    {
        var others = await _context.RequestBroadcasts
            .Where(b => b.RequestId == sessionId && b.TeacherId != teacherId)
            .ToListAsync();

        _context.RequestBroadcasts.RemoveRange(others);
    }

    public async Task<List<teacherRequestDto>> GetForTeacher(string teacherId)
    {
        return await _context.RequestBroadcasts
            .Where(b => b.TeacherId == teacherId)
            .Include(b => b.SessionRequest)
                .ThenInclude(r => r.Student)
                    .ThenInclude(s => s.ApplicationUser)  // Get student's user info
            .Include(b => b.SessionRequest)
                .ThenInclude(r => r.Subject)
            .Select(b => new teacherRequestDto
            {
                sessionId = b.RequestId,
                studentName =
                    b.SessionRequest.Student.ApplicationUser.FirstName + " " +
                    b.SessionRequest.Student.ApplicationUser.LastName,
                subject = b.SessionRequest.Subject.name,
                preferredDate = b.SessionRequest.PreferredDate,
                description = b.SessionRequest.Description,
                isAccepted = b.IsAccepted
            })
            .ToListAsync();
    }

}
