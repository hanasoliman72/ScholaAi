using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;

namespace ScholaAi.Services.Base
{
    public interface ISessionStreamService
    {
        Task<SessionDetailsDto> GetSessionById(int sessionId);
        Task<StartSessionResponseDto> StartSession(string teacherId, int sessionId);
        Task<StartSessionResponseDto> JoinSession(string studentId, int sessionId);
        Task<List<studentSessionDto>> GetStudentSessions(string studentId);
        Task EndSession(string teacherId, int sessionId, int focusScore = 0);
        Task SaveRecording(string teacherId, int sessionId, string recordingUrl, int duration);
        Task GenerateSummaryAsync(int sessionId, string videoUrl);
        Task ReportFocusAsync(string studentId, int sessionId, int focusScore);
        Task NotifyDistractionAsync(string studentId, int sessionId, string roomId, string reason);
    }
}
