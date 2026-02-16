using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.sessions
{
    public class sessionRequestRepository : ISessionRequestRepository
    {
        private readonly DBcontext _context;

        public sessionRequestRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task Add(SessionRequest request)
        {
            await _context.SessionRequests.AddAsync(request);
        }

        public async Task<SessionRequest?> GetById(int id)
        {
            return await _context.SessionRequests
                .Include(r => r.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .Include(r => r.Student)
                    .ThenInclude(s => s.ApplicationUser)
                .Include(r => r.Subject)
                .Include(r => r.RequestBroadcasts)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<List<SessionRequest>> GetForStudent(string studentId)
        {
            return await _context.SessionRequests
                .Include(r => r.Teacher)
                    .ThenInclude(t => t!.ApplicationUser)
                .Include(r => r.Subject)
                .Include(r => r.RequestBroadcasts)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
