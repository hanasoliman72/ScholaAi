using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IRatingRepository : IGenericRepository<Models.Rating>
    {
        Task<Models.Rating?> getByIdAsync(int ratingId);
        Task<Models.Rating?> getBySessionIdAsync(int sessionId);
        Task<IEnumerable<Models.Rating>> getByTeacherIdAsync(string teacherId);
        Task<decimal> getTeacherAverageRatingAsync(string teacherId);
        Task<bool> ratingExistsBySessionAsync(int sessionId);
    }
}
