using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Rating
{
    public class ratingRepository : genericRepository<rating>, IRatingRepository
    {
        public ratingRepository(DBcontext context) : base(context)
        {
        }

        // GET RATING FOR SPECIFIC SESSION
        public async Task<rating?> getBySessionIdAsync(int sessionId)
        {
            var rating = await _dbSet
                .Include(r => r.session)
                .Include(r => r.teacher)
                .FirstOrDefaultAsync(r => r.sessionId == sessionId);

            return rating;
        }

        // GET ALL RATINGS FOR A TEACHER
        public async Task<IEnumerable<rating>> getByTeacherIdAsync(int teacherId)
        {
            return await _dbSet
                .Where(r => r.teacherId == teacherId)
                .Include(r => r.session)
                .Include(r => r.teacher)
                .ToListAsync();
        }

        // GET AVERAGE RATING FOR A TEACHER
        public async Task<decimal> getTeacherAverageRatingAsync(int teacherId)
        {
            var ratings = await _dbSet
                .Where(r => r.teacherId == teacherId)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return (decimal)ratings.Average(r => r.ratingValue);
        }

        // CHECK IF A SESSION ALREADY HAS A RATING
        public async Task<bool> ratingExistsBySessionAsync(int sessionId)
        {
            return await _dbSet
                .AnyAsync(r => r.sessionId == sessionId);
        }
    }
}
