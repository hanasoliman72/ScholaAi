using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using System;

namespace ScholaAi.Repositories.Student
{
    public class studentRepository : genericRepository<student>, IStudentRepository
    {
        public studentRepository(DBcontext context) : base(context) { }

        public override async Task<student?> getByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.user)
                .FirstOrDefaultAsync(s => s.userId == id);
        }
    }
}
