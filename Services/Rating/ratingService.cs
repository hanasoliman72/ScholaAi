using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;
using ScholaAi.DTOs.Rating;
using ScholaAi.Models;
using Microsoft.EntityFrameworkCore;

namespace ScholaAi.Services.Rating
{
    public class ratingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly DBcontext _context;
        private readonly ILogger<ratingService> _logger;

        public ratingService(
            IRatingRepository ratingRepository,
            DBcontext context,
            ILogger<ratingService> logger)
        {
            _ratingRepository = ratingRepository;
            _context = context;
            _logger = logger;
        }

        // HELPER METHOD TO MAP FROM Rating → ratingDto
        private static ratingDto mapToDto(Models.Rating rating)
        {
            return new ratingDto
            {
                ratingId = rating.RatingId,
                sessionId = rating.SessionId,
                teacherId = rating.TeacherId,
                studentId = rating.StudentId,
                ratingValue = rating.RatingValue,
                comment = rating.Comment,
                createdAt = rating.CreatedAt
            };
        }

        public async Task<ratingDto> createRatingAsync(int sessionId, string? studentId, ratingCreateDto dto)
        {
            try
            {
                // Validate Session exists
                var session = await _context.Sessions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (session == null)
                    throw new InvalidOperationException("Session not found");

                // If authenticated (not anonymous), validate ownership
                if (!string.IsNullOrEmpty(studentId) && session.StudentId != studentId)
                    throw new UnauthorizedAccessException("You cannot rate a Session that doesn't belong to you");

                //// Validate Session has ended
                //if (Session.endTime > DateTime.UtcNow)
                //    throw new InvalidOperationException("You can only rate a Session after it has ended");

                // Check if Rating already exists
                var existingRating = await _ratingRepository.ratingExistsBySessionAsync(sessionId);
                if (existingRating)
                    throw new InvalidOperationException("You have already rated this Session");

                // Create the Rating
                var rating = new Models.Rating
                {
                    SessionId = sessionId,
                    StudentId = studentId,  // Can be null for anonymous
                    TeacherId = session.TeacherId,
                    RatingValue = dto.ratingValue,
                    Comment = dto.comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _ratingRepository.AddAsync(rating);
                _logger.LogInformation("Rating created for Session {SessionId}", sessionId);

                return mapToDto(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Rating for Session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<ratingDto?> updateRatingAsync(int ratingId, string? studentId, ratingUpdateDto dto)
        {
            try
            {
                // Validate Rating exists
                var existingRating = await _ratingRepository.getByIdAsync(ratingId);
                if (existingRating == null)
                    throw new InvalidOperationException("Rating not found");

                // Validate ownership (if not anonymous)
                if (!string.IsNullOrEmpty(studentId) && existingRating.StudentId != studentId)
                    throw new UnauthorizedAccessException("You can only update your own ratings");

                // Update the Rating
                existingRating.RatingValue = dto.ratingValue;
                existingRating.Comment = dto.comment;
                
                await _ratingRepository.updateAsync(existingRating);
                _logger.LogInformation("Rating {RatingId} updated", ratingId);

                return mapToDto(existingRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<bool> deleteRatingAsync(int ratingId, string? studentId)
        {
            try
            {
                // Validate Rating exists
                var existingRating = await _ratingRepository.getByIdAsync(ratingId);
                if (existingRating == null)
                    throw new InvalidOperationException("Rating not found");

                // Validate ownership (if not anonymous)
                if (!string.IsNullOrEmpty(studentId) && existingRating.StudentId != studentId)
                    throw new UnauthorizedAccessException("You can only delete your own ratings");

                await _ratingRepository.deleteAsync(existingRating);
                _logger.LogInformation("Rating {RatingId} deleted", ratingId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<ratingDto?> getRatingByIdAsync(int ratingId)
        {
            try
            {
                var rating = await _ratingRepository.getByIdAsync(ratingId);
                return rating != null ? mapToDto(rating) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<ratingDto?> getSessionRatingAsync(int sessionId)
        {
            try
            {
                var rating  = await _ratingRepository.getBySessionIdAsync(sessionId);
                return mapToDto(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Rating for Session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<ratingDto>> getTeacherRatingsAsync(string teacherId)
        {
            try
            {
                var ratings = await _ratingRepository.getByTeacherIdAsync(teacherId);
                return ratings.Select(r => new ratingDto
                {
                    ratingId = r.RatingId,
                    sessionId = r.SessionId,
                    teacherId = r.TeacherId,
                    studentId = r.StudentId,
                    ratingValue = r.RatingValue,
                    comment = r.Comment,
                    createdAt = r.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for Teacher {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<teacherAverageRatingDto> getTeacherAverageRatingAsync(string teacherId)
        {
            try
            {
                var averageRating = await _ratingRepository.getTeacherAverageRatingAsync(teacherId);
                var allRatings = await _ratingRepository.getByTeacherIdAsync(teacherId);

                return new teacherAverageRatingDto
                {
                    teacherId = teacherId,
                    averageRating = Math.Round(averageRating, 2),
                    totalRatings = allRatings.Count()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average Rating for Teacher {TeacherId}", teacherId);
                throw;
            }
        }
    }
}
