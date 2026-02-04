using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Teacher
{
    public class teacherRepository : genericRepository<teacher>, ITeacherRepository
    {
        public teacherRepository(DBcontext context) : base(context)
        {
        }

        public override async Task addAsync(teacher entity)
        {
            await base.addAsync(entity);
        }

        // ✅ Get teacher by id ومعاه user (شغالة زي ما هي)
        public async Task<teacher?> getByIdWithUserAsync(int teacherId)
        {
            return await _dbSet
                .Include(t => t.user)
                .FirstOrDefaultAsync(t => t.userId == teacherId);
        }

        // ✅ Search teachers (Student search)
        public async Task<List<teacher>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword)
        {
            var query = _dbSet
                .Include(t => t.user)
                .Include(t => t.teacherSubjects)
                    .ThenInclude(ts => ts.subject)
                .AsQueryable();

            // 🔍 search by teacher name
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(t =>
                    t.user.userName.Contains(name));
            }

            // 🔍 search by subject
            if (!string.IsNullOrWhiteSpace(subject))
            {
                query = query.Where(t =>
                    t.teacherSubjects.Any(ts =>
                        ts.subject.name.Contains(subject)));
            }

            // 🔍 search by keyword (college / experience)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.college.Contains(keyword) ||
                    t.teachingExperience.Contains(keyword));
            }

            return await query.ToListAsync();
        }
    }
}
