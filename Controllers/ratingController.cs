using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Rating;
using ScholaAi.Models;
using ScholaAi.Services.Base;
using ScholaAi.Mappings;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ratingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly ILogger<ratingController> _logger;

        public ratingController(IRatingService ratingService, ILogger<ratingController> logger)
        {
            _ratingService = ratingService;
            _logger = logger;
        }

        // HELPER METHOD TO GET STUDENT ID FROM TOKEN
        private int? getStudentIdFromToken()
        {
            var studentIdString = User.FindFirst("userId")?.Value;
            if (studentIdString == null) return null;

            return int.TryParse(studentIdString, out var studentId) ? studentId : null;
        }

        // POST: api/rating/{sessionId}
        [HttpPost("{sessionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> createRating(int sessionId, [FromBody] ratingCreateDto ratingCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            config.AssertConfigurationIsValid();
            try
            {
                var studentId = getStudentIdFromToken();
                var result = await _ratingService.createRatingAsync(sessionId, studentId, ratingCreateDto);

                // Return status 201 Created(not 200 OK) - tells client a resource was created
                // Add Location header -shows where to find the new resource(/ api / ratings / 12)
                // Return the created resource -includes all the details in the response body
                return CreatedAtAction(nameof(getRatingById), new { ratingId = result.ratingId }, new
                {
                    success = true,
                    message = "Rating submitted successfully",
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while creating rating");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rating for session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while submitting the rating"
                });
            }
        }

        // PUT: api/rating/{ratingId}
        [HttpPut("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> updateRating(int ratingId, [FromBody] ratingUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var studentId = getStudentIdFromToken();
                
                var result = await _ratingService.updateRatingAsync(ratingId, studentId, dto);

                if (result == null)
                    return NotFound(new { success = false, message = "Rating not found" });
                  
                return Ok(new
                {
                    success = true,
                    message = "Rating updated successfully",
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while updating rating {RatingId}", ratingId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the rating"
                });
            }
        }

        // DELETE: api/rating/{ratingId}
        [HttpDelete("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> deleteRating(int ratingId)
        {
            try
            {
                var studentId = getStudentIdFromToken();

                var result = await _ratingService.deleteRatingAsync(ratingId, studentId);

                if (!result)
                    return NotFound(new { success = false, message = "Rating not found" });

                return Ok(new { success = true, message = "Rating deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while deleting rating {RatingId}", ratingId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the rating"
                });
            }
        }

        // GET: api/rating/{ratingId}
        [HttpGet("{ratingId}")]
        [AllowAnonymous]
        public async Task<IActionResult> getRatingById(int ratingId)
        {
            try
            {
                var rating = await _ratingService.getRatingByIdAsync(ratingId);

                if (rating == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Rating not found"
                    });

                return Ok(new{success = true,data = rating});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the rating"
                });
            }
        }

        // GET: api/rating/teacher/{teacherId}/all
        [HttpGet("teacher/{teacherId}/all")]
        [AllowAnonymous]
        public async Task<IActionResult> getTeacherRatings(int teacherId)
        {
            try
            {
                var ratings = await _ratingService.getTeacherRatingsAsync(teacherId);

                return Ok(new{success = true,totalRatings = ratings.Count(),data = ratings});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for teacher {TeacherId}", teacherId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving teacher ratings"
                });
            }
        }

        // GET: api/rating/teacher/{teacherId}/average
        [HttpGet("teacher/{teacherId}/average")]
        [AllowAnonymous]
        public async Task<IActionResult> getTeacherAverageRating(int teacherId)
        {
            try
            {
                var result = await _ratingService.getTeacherAverageRatingAsync(teacherId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average rating for teacher {TeacherId}", teacherId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the average rating"
                });
            }
        }
    }
}
