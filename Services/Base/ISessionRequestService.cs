using ScholaAi.DTOs.Sessions;

namespace ScholaAi.Services.Base
{
    public interface ISessionRequestService
    {
        Task CreateRequest(int studentId, createSessionRequestDto dto);
        Task<List<teacherRequestDto>> GetTeacherRequests(int teacherId);
        Task AcceptRequest(int teacherId, int sessionId);
        Task RejectRequest(int teacherId, int sessionId);  
        Task<List<studentSessionDto>> GetStudentRequests(int teacherId);
    }
}
