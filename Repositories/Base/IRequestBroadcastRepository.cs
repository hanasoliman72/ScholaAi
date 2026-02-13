using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IRequestBroadcastRepository
    {
        Task Add(RequestBroadcast b);
        Task<List<teacherRequestDto>> GetForTeacher(string teacherId);
        Task Accept(string teacherId, int sessionId);
        Task Remove(string teacherId, int sessionId);
        Task RemoveOthers(int sessionId, string teacherId);

    }
}
