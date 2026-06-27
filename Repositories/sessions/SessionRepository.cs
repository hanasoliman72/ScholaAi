using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.sessions
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DBcontext _context;

        public SessionRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<Session?> GetByIdAsync(int sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Teacher).ThenInclude(t => t.ApplicationUser)
                .Include(s => s.Student).ThenInclude(s => s.ApplicationUser)
                .Include(s => s.SessionRequest).ThenInclude(sr => sr.Subject)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        public async Task<Session?> GetByRequestIdAsync(int requestId)
        {
            return await _context.Sessions
                .FirstOrDefaultAsync(s => s.RequestId == requestId);
        }

        public async Task<List<Session>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Sessions
                .Include(s => s.Teacher).ThenInclude(t => t.ApplicationUser)
                .Include(s => s.SessionRequest).ThenInclude(sr => sr.Subject)
                .Include(s => s.Rating)
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.StartedAt ?? s.SessionRequest.PreferredDate)
                .ToListAsync();
        }

        public async Task<List<Session>> GetByTeacherIdAsync(string teacherId)
        {
            return await _context.Sessions
                .Include(s => s.Student).ThenInclude(st => st.ApplicationUser)
                .Include(s => s.SessionRequest).ThenInclude(sr => sr.Subject)
                .Where(s => s.TeacherId == teacherId)
                .OrderByDescending(s => s.StartedAt ?? s.SessionRequest.PreferredDate)
                .ToListAsync();
        }


        public async Task<bool> HasActiveSessionForTeacherAsync(string teacherId)
        {
            return await _context.Sessions
                .AnyAsync(s => s.TeacherId == teacherId && s.Status == "active");
        }

        public async Task<bool> HasActiveSessionForStudentAsync(string studentId)
        {
            return await _context.Sessions
                .AnyAsync(s => s.StudentId == studentId && s.Status == "active");
        }

        public async Task AddAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}