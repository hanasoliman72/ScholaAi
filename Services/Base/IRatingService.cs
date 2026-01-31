using ScholaAi.DTOs.Rating;

namespace ScholaAi.Services.Base
{
    public interface IRatingService
    {
        Task<ratingDto> createRatingAsync(int sessionId,int? studentId, ratingCreateDto ratingCreateDTO);
        Task<ratingDto?> updateRatingAsync(int ratingId, int? studentId, ratingUpdateDto dto);
        Task<bool> deleteRatingAsync(int ratingId, int? studentId);
        Task<ratingDto?> getRatingByIdAsync(int ratingId);
        Task<ratingDto?> getSessionRatingAsync(int sessionId);
        Task<IEnumerable<ratingDto>> getTeacherRatingsAsync(int teacherId);
        Task<teacherAverageRatingDto> getTeacherAverageRatingAsync(int teacherId);
    }
}
