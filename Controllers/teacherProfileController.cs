using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Teacher;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetProfile(string teacherId)
        {
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
        public async Task<IActionResult> SearchTeachers(
            [FromQuery] string? name,
            [FromQuery] string? subject,
            [FromQuery] string? keyword)
        {
            var result = await _teacherProfileService
                .SearchTeachersAsync(name, subject, keyword);

            if (result == null || !result.Any())
                return NotFound("No teachers found");

            return Ok(result);
        }
    }
}
