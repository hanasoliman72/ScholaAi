using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Base;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : ControllerBase
    {
        private readonly IStudentDashboardService _dashboardService;

        public StudentDashboardController(IStudentDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/StudentDashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var dashboard = await _dashboardService.GetStudentDashboardAsync(userId);
            return Ok(new { success = true, data = dashboard });
        }
    }
}
