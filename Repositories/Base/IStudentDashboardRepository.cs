using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IStudentDashboardRepository
    {
        Task<Models.Student?> GetStudentDashboardAsync(string studentId);
    }
}
