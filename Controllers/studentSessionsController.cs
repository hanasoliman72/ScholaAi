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
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/studentSessions/{requestId}/join
        [HttpPost("{requestId}/join")]
        public async Task<IActionResult> Join(int requestId)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var result = await _sessionStreamService.JoinSession(studentId, requestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/studentSessions/{sessionId}/report-focus
        // Called by focus_server.py every ~30s with the current focus score.
        [HttpPost("{sessionId}/report-focus")]
        public async Task<IActionResult> ReportFocus(int sessionId, [FromBody] ReportFocusRequest req)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid token" });

            try
            {
                await _sessionStreamService.ReportFocusAsync(studentId, sessionId, req.FocusScore);
                return Ok(new { message = "Focus score updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/studentSessions/{sessionId}/notify-distraction
        // Called by focus_server.py when the student is repeatedly distracted.
        // The backend fires a SignalR DistractionAlert to the teacher.
        [HttpPost("{sessionId}/notify-distraction")]
        public async Task<IActionResult> NotifyDistraction(int sessionId, [FromBody] NotifyDistractionRequest req)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new { message = "Invalid token" });

            try
            {
                await _sessionStreamService.NotifyDistractionAsync(studentId, sessionId, req.RoomId, req.Reason);
                return Ok(new { message = "Distraction alert sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ReportFocusRequest
    {
        public int FocusScore { get; set; }
    }

    public class NotifyDistractionRequest
    {
        public string RoomId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
