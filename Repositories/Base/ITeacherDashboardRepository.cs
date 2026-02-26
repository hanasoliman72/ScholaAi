using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ITeacherDashboardRepository
    {
        Task<Models.Teacher?> GetTeacherDashboardAsync(string teacherId);
    }
}
