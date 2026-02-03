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

        public async Task Add(sessionRequest request)
        {
            await _context.sessionRequests.AddAsync(request);
        }

        public async Task<sessionRequest?> GetById(int id)
        {
            return await _context.sessionRequests
                .Include(r => r.teacher)
                    .ThenInclude(t => t.user)
                .Include(r => r.subject)
                .FirstOrDefaultAsync(r => r.requestId == id);
        }

        public async Task<List<sessionRequest>> GetForStudent(int studentId)
        {
            return await _context.sessionRequests
                .Include(r => r.teacher)
                    .ThenInclude(t => t.user)
                .Include(r => r.subject)
                .Where(r => r.studentId == studentId)
                .OrderByDescending(r => r.createdAt)
                .ToListAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
