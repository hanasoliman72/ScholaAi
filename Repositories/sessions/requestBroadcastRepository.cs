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
            .Where(b => b.TeacherId == teacherId && b.IsAccepted == false)
            .Include(b => b.TeacherSession)
                .ThenInclude(r => r.Student)  // Stop here - Student is already ApplicationUser
            .Include(b => b.TeacherSession)
                .ThenInclude(r => r.Subject)
            .Select(b => new teacherRequestDto
            {
                sessionId = b.RequestId,
                studentName =
                    b.TeacherSession.Student.FirstName + " " +  // Direct access
                    b.TeacherSession.Student.LastName,
                subject = b.TeacherSession.Subject.name,
                preferredDate = b.TeacherSession.PreferredDate,
                description = b.TeacherSession.Description
            })
            .ToListAsync();
    }

}
