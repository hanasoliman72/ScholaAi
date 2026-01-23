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
    }
}
