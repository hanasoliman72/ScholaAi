using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Rating
{
    public class ratingRepository : genericRepository<Models.Rating>, IRatingRepository
    {
        public ratingRepository(DBcontext context) : base(context)
        {
        }

        // GET BY ID (int key)
        public async Task<Models.Rating?> getByIdAsync(int ratingId)
        {
            return await _dbSet.FindAsync(ratingId);
        }

        // GET RATING FOR SPECIFIC SESSION
        public async Task<Models.Rating?> getBySessionIdAsync(int sessionId)
        {
            var rating = await _dbSet
                .Include(r => r.Session)
                .Include(r => r.Teacher)
                .FirstOrDefaultAsync(r => r.SessionId == sessionId);

            return rating;
        }

        // GET ALL RATINGS FOR A TEACHER
        public async Task<IEnumerable<Models.Rating>> getByTeacherIdAsync(string teacherId)
        {
            return await _dbSet
                .Where(r => r.TeacherId == teacherId)
                .Include(r => r.Session)
                .Include(r => r.Teacher)
                .Include(r => r.Student)
                .ToListAsync();
        }

        public Task<IEnumerable<Models.Rating>> getByTeacherIdAsync(int teacherId)
        {
            throw new NotImplementedException();
        }

        // GET AVERAGE RATING FOR A TEACHER
        public async Task<decimal> getTeacherAverageRatingAsync(string teacherId)
        {
            var ratings = await _dbSet
                .Where(r => r.TeacherId == teacherId)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return (decimal)ratings.Average(r => r.RatingValue);
        }

        public Task<decimal> getTeacherAverageRatingAsync(int teacherId)
        {
            throw new NotImplementedException();
        }

        // CHECK IF A SESSION ALREADY HAS A RATING
        public async Task<bool> ratingExistsBySessionAsync(int sessionId)
        {
            return await _dbSet
                .AnyAsync(r => r.SessionId == sessionId);
        }
    }
}
