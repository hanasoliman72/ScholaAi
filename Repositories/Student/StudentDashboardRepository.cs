using Microsoft.EntityFrameworkCore;
using ScholaAi.Repositories.Base;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Student
{
    public class StudentDashboardRepository : genericRepository<Models.Student>, IStudentDashboardRepository
    {
        public StudentDashboardRepository(DBcontext context) : base(context) { }

        public async Task<Models.Student?> GetStudentDashboardAsync(string studentId)
        {
            return await _context.Students
                .Include(s => s.ApplicationUser)
                    .ThenInclude(u => u.Wallet)
                        .ThenInclude(w => w.TransactionsFrom)
                .Include(s => s.Sessions)
                    .ThenInclude(sess => sess.SessionRequest)
                        .ThenInclude(sr => sr.Subject)
                .Include(s => s.Sessions)
                    .ThenInclude(sess => sess.Teacher)
                        .ThenInclude(t => t.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == studentId);
        }
    }
}
