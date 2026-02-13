using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.Student
{
    public class studentRepository : genericRepository<Models.Student>, IStudentRepository
    {
        public studentRepository(DBcontext context) : base(context) { }

        async Task<Models.Student?> GetByIdAsync(string id)
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
    }
}
