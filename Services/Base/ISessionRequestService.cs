using ScholaAi.DTOs.Sessions;

namespace ScholaAi.Services.Base
{
    public interface ISessionRequestService
    {
        Task CreateRequest(string studentId, createSessionRequestDto dto);
        Task<List<teacherRequestDto>> GetTeacherRequests(string teacherId);
        Task AcceptRequest(string teacherId, int sessionId);
        Task RejectRequest(string teacherId, int sessionId);
        Task<List<studentRequestDto>> GetStudentRequests(string studentId);
    }
}
