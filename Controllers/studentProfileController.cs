using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.Services;
using System.Formats.Asn1;

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
        public async Task<IActionResult> getProfile(int userId)
        {
            // TODO: Add authorization check - ensure user can only access their own profile
            var profile = await _studentProfileService.getStudentProfileAsync(userId);
            if(profile == null) 
                return NotFound("Student profile not found");

            return Ok(profile);
        }

        // PUT: api/studentProfile/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> updateProfile(int userId,[FromBody] updateStudentProfileDto dto)
        {

            if(!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentProfileService.updateStudentProfileAsync(userId, dto);
            if(!result) return NotFound("Student profile not found.");

            return Ok("Profile updated successfully");
        }

        // POST: api/studentProfile/{userId}/changePassword
        [HttpPost("{userId}/changePassword")]
        public async Task<IActionResult> changePassword(int userId,[FromBody] changePasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentProfileService.changePasswordAsync(userId, dto);
            if(!result) return BadRequest("Current password is incorrect or student not found.");

            return Ok("Password changed successfully");
        }

        // POST: api/studentProfile/{userId}/uploadPhoto
        [HttpPost("{userId}/uploadPhoto")]
        public async Task<IActionResult> uploadPhoto(int userId,IFormFile file) 
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var photoUrl = await _studentProfileService.uploadProfilePhotoAsync(userId,file);
            if(photoUrl == null) return NotFound("Student not found.");

            return Ok(new { photoUrl });
        }
    }
}
