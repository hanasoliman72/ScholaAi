using ScholaAi.DTOs.Sessions;
using ScholaAi.Models;

namespace ScholaAi.Services.Base
{
    public interface ISessionStreamService
    {
        Task<SessionDetailsDto> GetSessionById(int sessionId);
        Task<StartSessionResponseDto> StartSession(string teacherId, int sessionId);
        Task<StartSessionResponseDto> JoinSession(string studentId, int sessionId);
        Task EndSession(string teacherId, int sessionId, int focusScore = 0);
        Task SaveRecording(string teacherId, int sessionId, string recordingUrl, int duration);
        Task GenerateSummaryAsync(int sessionId, string videoUrl);
        /// <summary>Called by focus_server.py every ~30s to update the live focus score in the DB.</summary>
        Task ReportFocusAsync(string studentId, int sessionId, int focusScore);
        /// <summary>Called by focus_server.py to trigger a SignalR DistractionAlert to the teacher.</summary>
        Task NotifyDistractionAsync(string studentId, int sessionId, string roomId, string reason);
    }
}
