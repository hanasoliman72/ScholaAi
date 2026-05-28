using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Teacher
{
    public class TeacherDashboardRepository : genericRepository<Models.Teacher> , ITeacherDashboardRepository
    {
        public TeacherDashboardRepository(DBcontext context) : base(context) { }

        public async Task<Models.Teacher?> GetTeacherDashboardAsync(string teacherId)
        {
            return await _context.Teachers
                .Include(t => t.ApplicationUser)
                    .ThenInclude(u => u.Wallet)
                        .ThenInclude(w => w.TransactionsTo) 
                .Include(t => t.Sessions)
                    .ThenInclude(s => s.SessionRequest)
                        .ThenInclude(sr => sr.Subject)
                .Include(t => t.Sessions)
                    .ThenInclude(s => s.Student)
                        .ThenInclude(st => st.ApplicationUser)
                .Include(t => t.SessionRequests)                   
                    .ThenInclude(sr => sr.Subject)               
                .Include(t => t.SessionRequests)                   
                    .ThenInclude(sr => sr.Student)                 
                         .ThenInclude(st => st.ApplicationUser)     
                .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
        }
    }
}
