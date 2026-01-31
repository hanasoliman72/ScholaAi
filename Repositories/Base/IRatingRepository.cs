using ScholaAi.DTOs.Rating;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IRatingRepository : IGenericRepository<rating>
    {
        Task<ratingDto?> getBySessionIdAsync(int sessionId);
        Task<IEnumerable<ratingDto>> getByTeacherIdAsync(int teacherId);
        Task<decimal> getTeacherAverageRatingAsync(int teacherId);
        Task<bool> ratingExistsBySessionAsync(int sessionId);
    }
}
