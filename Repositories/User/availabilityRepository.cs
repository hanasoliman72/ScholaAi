using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.User
{
    public class availabilityRepository : genericRepository<availability>, IAvailabilityRepository
    {
        public availabilityRepository(DBcontext context) : base(context) { }

        public virtual async Task addRangeAsync(List<availability> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
