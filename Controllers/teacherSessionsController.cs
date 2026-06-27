using Microsoft.AspNetCore.Authorization;
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
        private readonly ISessionStreamService _sessionStreamService;
        private readonly IFileUploadService _fileService;

        public teacherSessionsController(ISessionRequestService sessionService, 
            ISessionStreamService sessionStream, IFileUploadService fileService)
        {
            _sessionService = sessionService;
            _sessionStreamService = sessionStream;
            _fileService = fileService;
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

        // GET: api/teacherSessions/{sessionId}
        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var session = await _sessionStreamService.GetSessionById(sessionId);
                if (session.TeacherId != teacherId)
                    return Forbid();
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/teacherSessions/{requestId}/start
        [HttpPost("{requestId}/start")]
        public async Task<IActionResult> Start(int requestId)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var result = await _sessionStreamService.StartSession(teacherId, requestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/teacherSessions/start-by-student/{studentId}
        [HttpPost("start-by-student/{studentId}")]
        public async Task<IActionResult> StartByStudent(string studentId)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                var result = await _sessionStreamService.StartSessionWithStudent(teacherId, studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/teacherSessions/{sessionId}/end
        [HttpPost("{sessionId}/end")]
        public async Task<IActionResult> End(int sessionId, [FromBody] EndSessionRequest req)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });
            try
            {
                await _sessionStreamService.EndSession(teacherId, sessionId, req.FocusScore);
                return Ok(new { message = "Session ended successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/teacherSessions/{sessionId}/upload-recording
        [HttpPost("{sessionId}/upload-recording")]
        [RequestSizeLimit(2_000_000_000)] // 2GB
        [RequestFormLimits(MultipartBodyLengthLimit = 2_000_000_000)] // 2GB
        public async Task<IActionResult> UploadRecording(int sessionId, [FromForm] UploadRecordingRequest request)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(new { message = "Invalid token" });

            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            try
            {
                var fileUrl = await _fileService.UploadToSupabaseAsync(request.File, "recordings");
                if (fileUrl == null)
                    return BadRequest(new { message = "Upload failed" });

                await _sessionStreamService.SaveRecording(teacherId, sessionId, fileUrl, request.Duration);

                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class UploadRecordingRequest
    {
        public IFormFile File { get; set; } = null!;
        public int Duration { get; set; }
    }

    public class EndSessionRequest
    {
        public int FocusScore { get; set; }
    }
}