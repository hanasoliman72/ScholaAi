using ScholaAi.DTOs.Rating;

namespace ScholaAi.Services.Base
{
    public interface IRatingService
    {
        Task<ratingDto> createRatingAsync(int sessionId,string? studentId, ratingCreateDto ratingCreateDTO);
        Task<ratingDto?> updateRatingAsync(int ratingId, string? studentId, ratingUpdateDto dto);
        Task<bool> deleteRatingAsync(int ratingId, string? studentId);
        Task<ratingDto?> getRatingByIdAsync(int ratingId);
        Task<ratingDto?> getSessionRatingAsync(int sessionId);
        Task<IEnumerable<ratingDto>> getTeacherRatingsAsync(string teacherId);
        Task<teacherAverageRatingDto> getTeacherAverageRatingAsync(string teacherId);
    }
}
