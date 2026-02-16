using Microsoft.AspNetCore.Identity;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> getByIdAsync(string id);
        Task<ApplicationUser?> getByEmailAsync(string email);
        Task<ApplicationUser> getUserByApplicationUserId(string appUserId);
        Task<ApplicationUser> getUserByUserNameAsync(string userName);
        Task<IdentityResult> resetPasswordAsync(ApplicationUser user, string token, string newPassword);

        Task<string> generatePasswordResetTokenAsync(ApplicationUser user);
    }

}
