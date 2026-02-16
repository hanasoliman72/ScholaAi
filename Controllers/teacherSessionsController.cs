using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Base;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Authorize(Roles = "Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class teacherSessionsController : ControllerBase
    {
        private readonly ISessionRequestService _sessionService;

        public teacherSessionsController(ISessionRequestService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet("GetMyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });

            var data = await _sessionService.GetTeacherRequests(teacherId);
            return Ok(data);
        }

        [HttpPost("{sessionId}/accept")]
        public async Task<IActionResult> Accept(int sessionId)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });

            try
            {
                await _sessionService.AcceptRequest(teacherId, sessionId);
                return Ok(new { message = "Request accepted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{sessionId}/reject")]
        public async Task<IActionResult> Reject(int sessionId)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });

            try
            {
                await _sessionService.RejectRequest(teacherId, sessionId);
                return Ok(new { message = "Request rejected successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}