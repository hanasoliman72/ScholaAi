using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;

namespace ScholaAi.Services.Base
{
    public interface ISessionStreamService
    {
        Task<Session> GetSessionById(int sessionId);
        Task<StartSessionResponseDto> StartSession(string teacherId, int sessionId);
        Task<StartSessionResponseDto> JoinSession(string studentId, int sessionId);
        Task EndSession(string teacherId, int sessionId);
    }
}
