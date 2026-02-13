using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Sessions;
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
            string teacherId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var data = await _sessionService.GetTeacherRequests(teacherId);
            return Ok(data);
        }
        [HttpPost("{sessionId}/accept")]
        public async Task<IActionResult> Accept(int sessionId)
        {
            string teacherId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _sessionService.AcceptRequest(teacherId, sessionId);
            return Ok(new { message = "Request accepted" });
        }
        [HttpPost("{sessionId}/reject")]
        public async Task<IActionResult> Reject(int sessionId)
        {
            string teacherId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _sessionService.RejectRequest(teacherId, sessionId);
            return Ok(new { message = "Request rejected" });
        }
    }
}
