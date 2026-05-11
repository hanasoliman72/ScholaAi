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
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        public async Task<Session?> GetByRequestIdAsync(int requestId)
        {
            return await _context.Sessions
                .FirstOrDefaultAsync(s => s.RequestId == requestId);
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