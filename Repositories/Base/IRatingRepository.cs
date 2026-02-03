using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IRatingRepository : IGenericRepository<rating>
    {
        Task<rating?> getBySessionIdAsync(int sessionId);
        Task<IEnumerable<rating>> getByTeacherIdAsync(int teacherId);
        Task<decimal> getTeacherAverageRatingAsync(int teacherId);
        Task<bool> ratingExistsBySessionAsync(int sessionId);
    }
}
