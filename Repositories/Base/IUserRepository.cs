using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IUserRepository : IGenericRepository<user>
    {
        Task<user?> getByEmailAsync(string email);
        Task<user> GetUserByApplicationUserId(string appUserId);

    }

}
