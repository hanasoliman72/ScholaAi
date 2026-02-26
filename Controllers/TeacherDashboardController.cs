using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScholaAi.Services.Base;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : ControllerBase
    {
        private readonly ITeacherDashboardService _dashboardService;

        public TeacherDashboardController(ITeacherDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/TeacherDashboard
        [HttpGet]
        public async Task<IActionResult> GetDashoard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid Token" });

            var dashboard = await _dashboardService.GetTeacherDashboardAsync(userId);
            return Ok(new { success = true, data = dashboard });
        }
    }
}
