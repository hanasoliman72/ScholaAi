using ScholaAi.DTOs.Student;

namespace ScholaAi.Services.Base
{
    public interface IStudentDashboardService
    {
        Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId);
    }
}
