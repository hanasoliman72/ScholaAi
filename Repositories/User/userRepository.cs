using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.User
{
    public class UserRepository : genericRepository<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBcontext _context;

        public UserRepository(DBcontext context, UserManager<ApplicationUser> userManager)
            : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> getByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser?> getByIdAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser?> getUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        // Identity handles this
        public async Task<IdentityResult> resetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<string> generatePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<ApplicationUser> getUserByApplicationUserId(string appUserId)
        {
            throw new NotImplementedException();
        }
    }
}
