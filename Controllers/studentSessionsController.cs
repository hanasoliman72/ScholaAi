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
        private readonly ISessionStreamService _sessionStreamService;

        public studentSessionsController(ISessionRequestService sessionService, ISessionStreamService sessionStream)
        {
            _sessionService = sessionService;
            _sessionStreamService = sessionStream;
        }

        // ============================
        // Create Session request
        // ============================
        [HttpPost("CreateRequest")]
        public async Task<IActionResult> Create([FromBody] createSessionRequestDto dto)
        {
            // 1️⃣ Check token
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return Unauthorized(new { message = "Invalid token" });

            string studentId = claim.Value;

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

            string studentId = claim.Value;

            var requests = await _sessionService.GetStudentRequests(studentId);

            return Ok(requests);
        }

        // GET: api/studentSessions/{sessionId}
        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var session = await _sessionStreamService.GetSessionById(sessionId);
                if (session.StudentId != studentId)
                    return Forbid();
                return Ok(new SessionDetailsDto
                {
                    SessionId = session.SessionId,
                    TeacherId = session.TeacherId,
                    StudentId = session.StudentId,
                    Status = session.Status,
                    RoomId = session.RoomId,
                    StartedAt = session.StartedAt,
                    EndedAt = session.EndedAt,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/teacherSessions/{sessionId}/join
        [HttpPost("{sessionId}/join")]
        public async Task<IActionResult> Join(int sessionId)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var result = await _sessionStreamService.JoinSession(studentId, sessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
