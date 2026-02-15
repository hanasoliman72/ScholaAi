using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.Services;
using System.Formats.Asn1;
using System.Security.Claims;

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
        public async Task<IActionResult> getProfile(string userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });
            
            var profile = await _studentProfileService.getStudentProfileAsync(userId);
            if(profile == null) 
                return NotFound("Student profile not found");

            return Ok(profile);
        }

        // PUT: api/studentProfile/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> updateProfile(string userId,[FromBody] updateStudentProfileDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message) = await _studentProfileService.updateStudentProfileAsync(userId, dto);
            if (!success)
                return BadRequest(message);

            return Ok(message);
        }

        // POST: api/studentProfile/{userId}/changePassword
        [HttpPost("{userId}/changePassword")]
        public async Task<IActionResult> changePassword(string userId,[FromBody] DTOs.Common.changePasswordDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentProfileService.changePasswordAsync(userId, dto);
            if(!result) return BadRequest("Current password is incorrect or Student not found.");

            return Ok("Password changed successfully");
        }

        // POST: api/studentProfile/{userId}/uploadPhoto
        [HttpPost("{userId}/uploadPhoto")]
        public async Task<IActionResult> uploadPhoto(string userId,IFormFile file) 
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var photoUrl = await _studentProfileService.uploadProfilePhotoAsync(userId,file);
            if(photoUrl == null) return NotFound("Student not found.");

            return Ok(new { photoUrl });
        }
    }
}
