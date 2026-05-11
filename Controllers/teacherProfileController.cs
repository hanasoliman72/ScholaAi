using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.Services.Teacher;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class teacherProfileController : ControllerBase
    {
        private readonly ITeacherProfileService _teacherProfileService;

        public teacherProfileController(ITeacherProfileService teacherProfileService)
        {
            _teacherProfileService = teacherProfileService;
        }

        // ===============================
        // ✅ Get Teacher Profile by ID
        // ===============================
        [HttpGet("{teacherId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetProfile(string teacherId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            if (userId != teacherId)
                return Unauthorized("You can only access your own profile");

            var profile = await _teacherProfileService.GetTeacherProfileAsync(teacherId);

            if (profile == null)
            {
                return NotFound("Teacher profile not found");
            }

            return Ok(profile);
        }

        // ===============================
        // ✅ Student Search Teachers
        // ===============================
        // api/teacherProfile/search?name=&Subject=&keyword=
        [HttpGet("search")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SearchTeachers(
            [FromQuery] string? name,
            [FromQuery] string? subject,
            [FromQuery] string? keyword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var result = await _teacherProfileService
                .SearchTeachersAsync(name, subject, keyword);

            if (result == null || !result.Any())
                return NotFound("No teachers found");

            return Ok(result);
        }
        // POST: api/studentProfile/{userId}/changePassword
        [HttpPost("{userId}/changePassword")]
        public async Task<IActionResult> changePassword(string userId, [FromBody] DTOs.Common.changePasswordDto dto)
        {
           
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userID))
                return Unauthorized(new { message = "Invalid token" });

            if (userId != userID)
                return Unauthorized("You can only access your own profile");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _teacherProfileService.ChangePasswordAsync(userId, dto);
            if (!result) return BadRequest("Current password is incorrect or Student not found.");

            return Ok("Password changed successfully");
        }
        [HttpPost("{userId}/uploadPhoto")]
        public async Task<IActionResult> uploadPhoto(string userId, IFormFile file)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            if (userId != userID)
                return Unauthorized("You can only access your own profile");

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var photoUrl = await _teacherProfileService.uploadProfilePhotoAsync(userId, file);
            if (photoUrl == null) return NotFound("Student not found.");

            return Ok(new { photoUrl });
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(string userId, [FromBody] updateTeacherProfileDto dto)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userID))
                return Unauthorized(new { message = "Invalid token" });

            if (userId != userID)
                return Unauthorized("You can only access your own profile");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _teacherProfileService.UpdateTeacherProfileAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // ===============================
        // GET My Students List
        // GET api/teacherProfile/myStudents?search=
        // ===============================
        [HttpGet("myStudents")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyStudents([FromQuery] string? search)
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized(new { message = "Invalid token" });

                var result = await _teacherProfileService
                    .GetMyStudentsAsync(teacherId, search);

                return Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching students"
                });
            }
        }

        // ===============================
        // GET Student Progress
        // GET api/teacherProfile/myStudents/{studentId}/progress
        // ===============================
        [HttpGet("myStudents/{studentId}/progress")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetStudentProgress(string studentId)
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized(new { message = "Invalid token" });

                var result = await _teacherProfileService
                    .GetStudentProgressAsync(teacherId, studentId);

                return result == null
                    ? NotFound(new { message = "Student not found or no sessions together" })
                    : Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching student progress"
                });
            }
        }
    }
}
