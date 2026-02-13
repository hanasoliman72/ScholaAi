using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Teacher
{
    public class teacherRepository : genericRepository<Models.Teacher>, ITeacherRepository
    {
        public teacherRepository(DBcontext context) : base(context)
        {
        }

        public override async Task AddAsync(Models.Teacher entity)
        {
            await base.AddAsync(entity);
        }

        public async Task<Models.Teacher?> getByIdWithUserAsync(string teacherId)
        {
            return await _dbSet
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
        }
        public async Task<List<Models.Teacher>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword)
        {
            var query = _dbSet
                .Include(t => t.ApplicationUser)
                .Include(t => t.Subject)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(t =>
                    t.ApplicationUser.UserName.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                query = query.Where(t =>
                    t.Subject.name.Contains(subject));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.College.Contains(keyword) ||
                    t.TeachingExperience.Contains(keyword));
            }

            return await query.ToListAsync();
        }
    }
}
