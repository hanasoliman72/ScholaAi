using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.User
{
    public class userRepository : genericRepository<user>, IUserRepository
    {
        private readonly UserManager<applicationUser> _userManager;
        public userRepository(DBcontext context, UserManager<applicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

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
        public async Task<user?> getUserByApplicationUserId(string applicationUserId)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.applicationUserId == applicationUserId);
        }

        public async Task<user> getUserByUserNameAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.userName == userName);
        }
        public async Task<IdentityResult> resetPasswordAsync(applicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        public async Task<string> generatePasswordResetTokenAsync(applicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
