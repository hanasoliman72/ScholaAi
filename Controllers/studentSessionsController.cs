using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.DTOs.Sessions;
using ScholaAi.Services.Base;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Authorize(Roles = "Student")]
    [ApiController]
    [Route("api/studentSessions")]
    public class studentSessionsController : ControllerBase
    {
        private readonly ISessionRequestService _sessionService;

        public studentSessionsController(ISessionRequestService sessionService)
        {
            _sessionService = sessionService;
        }

        // ============================
        // Create session request
        // ============================
        [HttpPost("CreateRequest")]
        public async Task<IActionResult> Create([FromBody] createSessionRequestDto dto)
        {
            // 1️⃣ Check token
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            int studentId = int.Parse(claim.Value);

            // 2️⃣ Check body
            if (dto == null)
                return BadRequest(new { message = "Request body is missing" });

            await _sessionService.CreateRequest(studentId, dto);

            return Ok(new
            {
                message = "The request was sent successfully to teachers"
            });
        }

        // ============================
        // Get my requests
        // ============================
        [HttpGet("GetMyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            int studentId = int.Parse(claim.Value);

            var requests = await _sessionService.GetStudentRequests(studentId);

            return Ok(requests);
        }
    }
}
