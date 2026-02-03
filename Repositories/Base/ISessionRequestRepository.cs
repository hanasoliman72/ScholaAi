using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ISessionRequestRepository
    {
        Task Add(sessionRequest request);
        Task<sessionRequest?> GetById(int id);
        Task<List<sessionRequest>> GetForStudent(int studentId);
        Task Save();
    }
}
