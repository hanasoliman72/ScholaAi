using AutoMapper;
using ScholaAi.DTOs.Rating;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.Rating
{
    public class ratingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IGenericRepository<session> _sessionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ratingService> _logger;

        public ratingService(
            IRatingRepository ratingRepository,
            IGenericRepository<session> sessionRepository,
            IMapper mapper,
            ILogger<ratingService> logger)
        {
            _ratingRepository = ratingRepository;
            _sessionRepository = sessionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ratingDto> createRatingAsync(int sessionId, int? studentId, ratingCreateDto dto)
        {
            try
            {
                // Validate session exists
                var session = await _sessionRepository.getByIdAsync(sessionId);
                if (session == null)
                    throw new InvalidOperationException("Session not found");

                // If authenticated (not anonymous), validate ownership
                if (studentId.HasValue && session.studentId != studentId)
                    throw new UnauthorizedAccessException("You cannot rate a session that doesn't belong to you");

                //// Validate session has ended
                //if (session.endTime > DateTime.UtcNow)
                //    throw new InvalidOperationException("You can only rate a session after it has ended");

                // Check if rating already exists
                var existingRating = await _ratingRepository.ratingExistsBySessionAsync(sessionId);
                if (existingRating)
                    throw new InvalidOperationException("You have already rated this session");

                // Create the rating
                var rating = new rating
                {
                    sessionId = sessionId,
                    studentId = studentId,  // Can be null for anonymous
                    teacherId = session.teacherId,
                    ratingValue = dto.ratingValue,
                    comment = dto.comment,
                    createdAt = DateTime.UtcNow
                };

                await _ratingRepository.addAsync(rating);
                _logger.LogInformation("Rating created for session {SessionId}", sessionId);

                return _mapper.Map<ratingDto>(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rating for session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<ratingDto?> updateRatingAsync(int ratingId, int? studentId, ratingUpdateDto dto)
        {
            try
            {
                // Validate rating exists
                var existingRating = await _ratingRepository.getByIdAsync(ratingId);
                if (existingRating == null)
                    throw new InvalidOperationException("Rating not found");

                // Validate ownership (if not anonymous)
                if (studentId.HasValue && existingRating.studentId != studentId)
                    throw new UnauthorizedAccessException("You can only update your own ratings");

                // Update the rating
                existingRating.ratingValue = dto.ratingValue;
                existingRating.comment = dto.comment;
                
                await _ratingRepository.updateAsync(existingRating);
                _logger.LogInformation("Rating {RatingId} updated", ratingId);

                return _mapper.Map<ratingDto>(existingRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<bool> deleteRatingAsync(int ratingId, int? studentId)
        {
            try
            {
                // Validate rating exists
                var existingRating = await _ratingRepository.getByIdAsync(ratingId);
                if (existingRating == null)
                    throw new InvalidOperationException("Rating not found");

                // Validate ownership (if not anonymous)
                if (studentId.HasValue && existingRating.studentId != studentId)
                    throw new UnauthorizedAccessException("You can only delete your own ratings");

                await _ratingRepository.deleteAsync(existingRating);
                _logger.LogInformation("Rating {RatingId} deleted", ratingId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<ratingDto?> getRatingByIdAsync(int ratingId)
        {
            try
            {
                var rating = await _ratingRepository.getByIdAsync(ratingId);
                return rating != null ? _mapper.Map<ratingDto>(rating) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating {RatingId}", ratingId);
                throw;
            }
        }

        public async Task<ratingDto?> getSessionRatingAsync(int sessionId)
        {
            try
            {
                return await _ratingRepository.getBySessionIdAsync(sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating for session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<ratingDto>> getTeacherRatingsAsync(int teacherId)
        {
            try
            {
                return await _ratingRepository.getByTeacherIdAsync(teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for teacher {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<teacherAverageRatingDto> getTeacherAverageRatingAsync(int teacherId)
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
                _logger.LogError(ex, "Error retrieving average rating for teacher {TeacherId}", teacherId);
                throw;
            }
        }
    }
}
