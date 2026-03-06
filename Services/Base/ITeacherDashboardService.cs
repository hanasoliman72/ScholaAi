using ScholaAi.DTOs.Teacher;

namespace ScholaAi.Services.Base
{
    public interface ITeacherDashboardService
    {
        Task<TeacherDashboardDto> GetTeacherDashboardAsync(string teacherId);
    }
}
