using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ISessionRequestRepository
    {
        Task Add(SessionRequest request);
        Task<SessionRequest?> GetById(int id);
        Task<List<SessionRequest>> GetForStudent(string studentId);
        Task Save();
    }
}
