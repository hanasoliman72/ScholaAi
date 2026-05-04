//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using ScholaAi.Models;
//using ScholaAi.Services.Base;
//using System.Security.Claims;

//namespace ScholaAi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class SessionStreamController : ControllerBase
//    {
//        private readonly ISessionStreamService _sessionStreamService;

//        public SessionStreamController(ISessionStreamService sessionStreamService)
//        {
//            _sessionStreamService = sessionStreamService;
//        }

//        // Teacher calls this → gets RoomId + role=host
//        // POST: api/SessionStream/{sessionId}/start
//        [HttpPost("{sessionId}/start")]
//        public async Task<IActionResult> Start(int sessionId)
//        {
//            //var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            //if (string.IsNullOrEmpty(userId))
//            //    return Unauthorized(new { message = "Invalid token" });

//            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(teacherId))
//                return Unauthorized(new { message = "Invalid token" });

//            var result = await _sessionStreamService.StartSession(teacherId, sessionId);
//            return Ok(result);
//        }

//        // Student calls this → gets RoomId + role=viewer
//        // POST: api/SessionStream/{sessionId}/join
//        [HttpPost("{sessionId}/join")]
//        public async Task<IActionResult> Join(int sessionId)
//        {
//            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(studentId))
//                return Unauthorized(new { message = "Invalid token" });

//            var result = await _sessionStreamService.JoinSession(studentId, sessionId);
//            return Ok(result);
//        }

//        // Teacher calls this when leaving
//        // POST: api/SessionStream/{sessionId}/end
//        [HttpPost("{sessionId}/end")]
//        public async Task<IActionResult> End(int sessionId, [FromBody] EndSessionRequest req)
//        {
//            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(teacherId))
//                return Unauthorized(new { message = "Invalid token" });

//            await _sessionStreamService.EndSession(teacherId, sessionId, req.FocusScore);
//            return Ok();
//        }
//    }

//    public class EndSessionRequest
//    {
//        public int FocusScore { get; set; }
//    }
//}
