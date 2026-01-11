using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.Student
{
    public class studentRepository : genericRepository<student>, IStudentRepository
    {
        public studentRepository(DBcontext context) : base(context) { }

        public override async Task<student?> getByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.user)
                    .ThenInclude(u => u.wallet) 
                        .ThenInclude(w => w.transactionsFrom) // Transactions FROM (payments)
                            .ThenInclude(sess => sess.session)
                                .ThenInclude(t => t.teacher)
                                    .ThenInclude(tu => tu.user)
                .Include(s => s.sessions)
                    .ThenInclude(sess => sess.transaction) // Session transactions
                .FirstOrDefaultAsync(s => s.userId == id);
        }
    }
}
