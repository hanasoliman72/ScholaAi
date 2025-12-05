using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Student;
using ScholaAi.Services;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class studentProfileController : ControllerBase
    {
        private readonly IStudentProfileService _studentProfileService;

        public studentProfileController(IStudentProfileService studentProfileService)
        {
            _studentProfileService = studentProfileService;
        }

        // GET: api/studentProfile/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<studentProfileDto>> getProfile(int userId)
        {
            // TODO: Add authorization check - ensure user can only access their own profile
            var profile = await _studentProfileService.getStudentProfileAsync(userId);
            if(profile == null) 
                return NotFound(new {message = "Student profile not found" });

            return Ok(profile);
        }

        // PUT: api/studentProfile/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> updateStudentProfile(int userId,[FromBody] updateStudentProfileDto dto)
        {

            if(!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentProfileService.updateStudentProfileAsync(userId, dto);
            if(!result) return NotFound("Student profile not found.");

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}
