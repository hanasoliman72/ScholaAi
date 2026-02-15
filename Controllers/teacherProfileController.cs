using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
