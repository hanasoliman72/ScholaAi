using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IRequestBroadcastRepository
    {
        Task Add(requestBroadcast b);
        Task<List<teacherRequestDto>> GetForTeacher(int teacherId);
        Task Accept(int teacherId, int sessionId);
        Task Remove(int teacherId, int sessionId);
        Task RemoveOthers(int sessionId, int teacherId);

    }
}
