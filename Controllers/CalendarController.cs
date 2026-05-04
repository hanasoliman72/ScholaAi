using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Calendar;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        // ═══════════════════════════════════════════════════════
        // STUDENT ENDPOINTS
        // ═══════════════════════════════════════════════════════

        // GET api/Calendar/student/month?year=2025&month=10
        [HttpGet("student/month")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentMonth(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(studentId))
                    return Unauthorized();

                // Default to current month if not provided
                if (year == 0) year = DateTime.UtcNow.Year;
                if (month == 0) month = DateTime.UtcNow.Month;

                var result = await _calendarService
                    .GetStudentMonthAsync(studentId, year, month);

                return Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching calendar"
                });
            }
        }

        // GET api/Calendar/student/day?date=2025-10-24
        [HttpGet("student/day")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentDay([FromQuery] DateTime date)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(studentId))
                    return Unauthorized();

                if (date == default)
                    date = DateTime.UtcNow.Date;

                var result = await _calendarService
                    .GetStudentDayAsync(studentId, date);

                return Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching day sessions"
                });
            }
        }

        // GET api/Calendar/student/session/{sessionId}/notes
        [HttpGet("student/session/{sessionId}/notes")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetSessionNotes(int sessionId)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(studentId))
                    return Unauthorized();

                var notes = await _calendarService.GetSessionNotesAsync(studentId, sessionId);
                return notes == null
                    ? NotFound(new { message = "Session not found" })
                    : Ok(new { success = true, data = notes });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching session notes"
                });
            }
        }

        // ═══════════════════════════════════════════════════════
        // TEACHER ENDPOINTS
        // ═══════════════════════════════════════════════════════

        // GET api/Calendar/teacher/month?year=2025&month=10
        [HttpGet("teacher/month")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherMonth(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized();

                if (year == 0) year = DateTime.UtcNow.Year;
                if (month == 0) month = DateTime.UtcNow.Month;

                var result = await _calendarService
                    .GetTeacherMonthAsync(teacherId, year, month);

                return Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching calendar"
                });
            }
        }

        // GET api/Calendar/teacher/day?date=2025-10-24
        [HttpGet("teacher/day")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherDay([FromQuery] DateTime date)
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized();

                if (date == default)
                    date = DateTime.UtcNow.Date;

                var result = await _calendarService
                    .GetTeacherDayAsync(teacherId, date);

                return Ok(new { success = true, data = result });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching day sessions"
                });
            }
        }

        // GET api/Calendar/teacher/session/{sessionId}/analysis
        [HttpGet("teacher/session/{sessionId}/analysis")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetSessionAnalysis(int sessionId)
        {
            try
            {
                var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(teacherId))
                    return Unauthorized();

                var analysis = await _calendarService
                    .GetSessionAnalysisAsync(teacherId, sessionId);

                return analysis == null
                    ? NotFound(new { message = "Session not found" })
                    : Ok(new { success = true, data = analysis });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching session analysis"
                });
            }
        }
    }
}