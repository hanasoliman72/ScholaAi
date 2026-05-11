using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface ITeacherRepository : IGenericRepository<Models.Teacher>
    {
        // Existing methods
        // ✅ تجيب Teacher ومعاه بيانات ApplicationUser
        Task<Models.Teacher?> getByIdWithUserAsync(string teacherId);

        // ✅ search عن Teacher بالاسم أو المادة أو كلمة مفتاحية
        Task<List<Models.Teacher>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword);

        // ✅ NEW - My Students
        Task<List<Models.Session>> GetTeacherSessionsWithStudentsAsync(string teacherId);
        Task<List<Models.Session>> GetStudentSessionsWithTeacherAsync(
            string teacherId, string studentId);
    }
}