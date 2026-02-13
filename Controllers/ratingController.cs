using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Rating;
using ScholaAi.Services.Base;
using System.Security.Claims;

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

        // POST: api/Rating/{sessionId}
        [HttpPost("{sessionId}")]
        [Authorize]
        public async Task<IActionResult> createRating(int sessionId, [FromBody] ratingCreateDto ratingCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
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
                _logger.LogWarning(ex, "Unauthorized access while creating Rating");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Rating for Session {SessionId}", sessionId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while submitting the Rating"
                });
            }
        }

        // PUT: api/Rating/{ratingId}
        [HttpPut("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> updateRating(int ratingId, [FromBody] ratingUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

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
                _logger.LogWarning(ex, "Unauthorized access while updating Rating {RatingId}", ratingId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the Rating"
                });
            }
        }

        // DELETE: api/Rating/{ratingId}
        [HttpDelete("{ratingId}")]
        [Authorize]
        public async Task<IActionResult> deleteRating(int ratingId)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                var result = await _ratingService.deleteRatingAsync(ratingId, studentId);

                if (!result)
                    return NotFound(new { success = false, message = "Rating not found" });

                return Ok(new { success = true, message = "Rating deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while deleting Rating {RatingId}", ratingId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the Rating"
                });
            }
        }

        // GET: api/Rating/{ratingId}
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
                _logger.LogError(ex, "Error retrieving Rating {RatingId}", ratingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the Rating"
                });
            }
        }

        // GET: api/Rating/Teacher/{teacherId}/all
        [HttpGet("Teacher/{teacherId}/all")]
        [AllowAnonymous]
        public async Task<IActionResult> getTeacherRatings(string teacherId)
        {
            try
            {
                var ratings = await _ratingService.getTeacherRatingsAsync(teacherId);

                return Ok(new{success = true,totalRatings = ratings.Count(),data = ratings});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ratings for Teacher {TeacherId}", teacherId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving Teacher ratings"
                });
            }
        }

        // GET: api/Rating/Teacher/{teacherId}/average
        [HttpGet("Teacher/{teacherId}/average")]
        [AllowAnonymous]
        public async Task<IActionResult> getTeacherAverageRating(string teacherId)
        {
            try
            {
                var result = await _ratingService.getTeacherAverageRatingAsync(teacherId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving average Rating for Teacher {TeacherId}", teacherId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the average Rating"
                });
            }
        }
    }
}
