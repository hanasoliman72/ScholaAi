using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Student;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.Student
{
    public class studentRepository : genericRepository<Models.Student>, IStudentRepository, IStudentDashboardRepository
    {
        public studentRepository(DBcontext context) : base(context) { }

        public async Task<Models.Student?> GetByIdAsync(string id)
        {
            return await _dbSet
                .Include(s => s.ApplicationUser)
                    .ThenInclude(u => u.Wallet)
                        .ThenInclude(w => w.TransactionsFrom) // Transactions FROM (payments)
                            .ThenInclude(sess => sess.Session)
                                .ThenInclude(t => t.Teacher)
                                    .ThenInclude(tu => tu.ApplicationUser)
                .Include(s => s.Sessions)
                    .ThenInclude(sess => sess.Transaction) // Session transactions
                .FirstOrDefaultAsync(s => s.ApplicationUserId == id);
        }

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
