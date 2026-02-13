using ScholaAi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScholaAi.Repositories.Base
{
    public interface ITeacherRepository : IGenericRepository<Models.Teacher>
    {
        // ✅ تجيب Teacher ومعاه بيانات ApplicationUser
        Task<Models.Teacher?> getByIdWithUserAsync(string teacherId);

        // ✅ search عن Teacher بالاسم أو المادة أو كلمة مفتاحية
        Task<List<Models.Teacher>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword);
    }
}
