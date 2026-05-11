using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ISessionRepository
    {
        Task<Session?> GetByIdAsync(int sessionId);
        Task<Session?> GetByRequestIdAsync(int requestId);
        Task AddAsync(Session session);
        Task SaveAsync();
    }
}
