using ScholaAi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScholaAi.Repositories.Base
{
    public interface ITeacherRepository : IGenericRepository<teacher>
    {
        // ✅ تجيب teacher ومعاه بيانات user
        Task<teacher?> getByIdWithUserAsync(int teacherId);

        // ✅ search عن teacher بالاسم أو المادة أو كلمة مفتاحية
        Task<List<teacher>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword);
    }
}
