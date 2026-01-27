using Microsoft.AspNetCore.Identity;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IUserRepository : IGenericRepository<user>
    {
        Task<user?> getByEmailAsync(string email);
        Task<user> getUserByApplicationUserId(string appUserId);
        Task<user> getUserByUserNameAsync(string userName);
        Task<IdentityResult> resetPasswordAsync(applicationUser user, string token, string newPassword);

        Task<string> generatePasswordResetTokenAsync(applicationUser user);
    }

}
