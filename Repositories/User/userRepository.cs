using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.User
{
    public class userRepository : genericRepository<user>, IUserRepository
    {
        public userRepository(DBcontext context) : base(context) { }

        public async Task<user?> getByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.email == email);
        }

        // include student/teacher:
        public override async Task<user?> getByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.student)
                .Include(u => u.teacher)
                .FirstOrDefaultAsync(u => u.userId == id);
        }
    }
}
