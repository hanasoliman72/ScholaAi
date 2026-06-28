
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScholaAi.DTOs.Admin;
using ScholaAi.Models;
using ScholaAi.Services.Admin;
using ScholaAi.Services.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _adminService = adminService;
            _userManager = userManager;
            _configuration = configuration;
        }

        // ═══════════════════════════════════════════════════════
        // AUTH
        // POST api/Admin/login
        // ═══════════════════════════════════════════════════════
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                    return Unauthorized(new { message = "Access denied. Admins only." });

                var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!passwordValid)
                    return Unauthorized(new { message = "Invalid email or password" });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:Secretkey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(8),
                    signingCredentials: credentials
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during login" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // PROFILE
        // GET    api/Admin/profile
        // PUT    api/Admin/profile
        // POST   api/Admin/profile/change-password
        // ═══════════════════════════════════════════════════════
        [HttpGet("profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();

                var profile = await _adminService.GetAdminProfileAsync(adminId);
                return profile == null
                    ? NotFound(new { message = "Admin not found" })
                    : Ok(new { success = true, data = profile });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching profile" });
            }
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProfile([FromBody] AdminEditProfileDto dto)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();

                var result = await _adminService.UpdateAdminProfileAsync(adminId, dto);
                return result
                    ? Ok(new { success = true, message = "Profile updated successfully" })
                    : NotFound(new { message = "Admin not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating profile" });
            }
        }

        [HttpPost("profile/change-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword([FromBody] AdminChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();

                var result = await _adminService.ChangeAdminPasswordAsync(adminId, dto);
                return result
                    ? Ok(new { success = true, message = "Password changed successfully" })
                    : BadRequest(new { success = false, message = "Current password is incorrect" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while changing password" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // DASHBOARD
        // GET api/Admin/dashboard
        // ═══════════════════════════════════════════════════════
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var data = await _adminService.GetDashboardAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching dashboard" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // USERS
        // ═══════════════════════════════════════════════════════
        // GET api/Admin/users?search=&role=
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? search,
            [FromQuery] string? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync(search, role);
                var totalCount = users.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var paged = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    success = true,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    page = page,
                    pageSize = pageSize,
                    data = paged
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching users" });
            }
        }

        // GET api/Admin/users/{userId}
        [HttpGet("users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var user = await _adminService.GetUserDetailAsync(userId);
                return user == null
                    ? NotFound(new { message = "User not found" })
                    : Ok(new { success = true, data = user });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching user" });
            }
        }

        // POST api/Admin/users
        [HttpPost("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _adminService.CreateUserAsync(dto);
                return Ok(new { success = true, data = created });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT api/Admin/users/{userId}
        [HttpPut("users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(
            string userId,
            [FromBody] AdminEditUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _adminService.EditUserAsync(userId, dto);
                return result
                    ? Ok(new { success = true, message = "User updated successfully" })
                    : NotFound(new { message = "User not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating user" });
            }
        }

        // DELETE api/Admin/users/{userId}
        [HttpDelete("users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // ✅ Prevent admin from deleting themselves
                if (adminId == userId)
                    return BadRequest(new { success = false, message = "You cannot delete your own account" });

                var result = await _adminService.DeleteUserAsync(adminId, userId);
                return result
                    ? Ok(new { success = true, message = "User deleted successfully" })
                    : NotFound(new { message = "User not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting user" });
            }
        }

        // PUT api/Admin/users/{userId}/role
        [HttpPut("users/{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userId, [FromBody] ChangeUserRoleDto dto)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _adminService.ChangeUserRoleAsync(adminId, userId, dto);
                return result
                    ? Ok(new { success = true, message = "Role updated successfully" })
                    : NotFound(new { message = "User not found" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while changing role" });
            }
        }


        // POST api/Admin/users/{userId}/suspend
        [HttpPost("users/{userId}/suspend")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Suspend(string userId, [FromBody] SuspendUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // ✅ Prevent admin from suspending themselves
                if (adminId == userId)
                    return BadRequest(new { success = false, message = "You cannot suspend your own account" });

                var result = await _adminService.SuspendUserAsync(adminId, userId, dto);
                return result
                    ? Ok(new { success = true, message = $"User suspended for {dto.DurationInDays} day(s)" })
                    : NotFound(new { message = "User not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while suspending user" });
            }
        }


        // POST api/Admin/users/{userId}/unsuspend
        [HttpPost("users/{userId}/unsuspend")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unsuspend(string userId)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _adminService.UnsuspendUserAsync(adminId, userId);
                return result
                    ? Ok(new { success = true, message = "User unsuspended successfully" })
                    : NotFound(new { message = "User not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while unsuspending user" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // VERIFY / UNVERIFY TEACHER
        // ═══════════════════════════════════════════════════════

        //POST api/Admin/users/{teacherId}/verify
        [HttpPost("users/{teacherId}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyTeacher(string teacherId, [FromBody] VerifyTeacherDto dto)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _adminService.VerifyTeacherAsync(adminId, teacherId, dto.Notes);
                return result
                    ? Ok(new { success = true, message = "Teacher verified successfully" })
                    : NotFound(new { message = "Teacher not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while verifying teacher" });
            }
        }


        //POST api/Admin/users/{teacherId}/unverify
        [HttpPost("users/{teacherId}/unverify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnverifyTeacher(string teacherId)
        {
            try
            {
                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _adminService.UnverifyTeacherAsync(adminId, teacherId);
                return result
                    ? Ok(new { success = true, message = "Teacher unverified successfully" })
                    : NotFound(new { message = "Teacher not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while unverifying teacher" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // SESSIONS
        // ═══════════════════════════════════════════════════════
        // GET api/Admin/sessions?search=
        [HttpGet("sessions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSessions(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var sessions = await _adminService.GetAllSessionsAsync(search);
                var totalCount = sessions.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var paged = sessions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    success = true,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    page = page,
                    pageSize = pageSize,
                    data = paged
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching sessions" });
            }
        }

        // ⚠️ IMPORTANT: /live must come BEFORE /{sessionId}
        // GET api/Admin/sessions/live
        [HttpGet("sessions/live")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLiveSessions()
        {
            try
            {
                var sessions = await _adminService.GetLiveSessionsAsync();
                return Ok(new { success = true, totalCount = sessions.Count, data = sessions });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching live sessions" });
            }
        }

        // GET api/Admin/sessions/{sessionId}
        [HttpGet("sessions/{sessionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            try
            {
                var session = await _adminService.GetSessionDetailAsync(sessionId);
                return session == null
                    ? NotFound(new { message = "Session not found" })
                    : Ok(new { success = true, data = session });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching session" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // PAYMENTS
        // ═══════════════════════════════════════════════════════

        // ⚠️ IMPORTANT: /export must come BEFORE /{transactionId}
        // GET api/Admin/payments/export
        [HttpGet("payments/export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportPayments()
        {
            try
            {
                var csvBytes = await _adminService.ExportPaymentsCsvAsync();
                return File(csvBytes, "text/csv",
                    $"payments_{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while exporting payments" });
            }
        }

        // GET api/Admin/payments?search=
        [HttpGet("payments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPayments(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var payments = await _adminService.GetAllPaymentsAsync(search);
                var totalCount = payments.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var paged = payments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    success = true,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    page = page,
                    pageSize = pageSize,
                    data = paged
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching payments" });
            }
        }

        // GET api/Admin/payments/{transactionId}
        [HttpGet("payments/{transactionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPayment(int transactionId)
        {
            try
            {
                var payment = await _adminService.GetPaymentDetailAsync(transactionId);
                return payment == null
                    ? NotFound(new { message = "Payment not found" })
                    : Ok(new { success = true, data = payment });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching payment" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // RATINGS
        // ═══════════════════════════════════════════════════════

        //GET api/Admin/ratings
        [HttpGet("ratings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRatings()
        {
            try
            {
                var ratings = await _adminService.GetAllRatingsAsync();
                return Ok(new { success = true, totalCount = ratings.Count, data = ratings });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching ratings" });
            }
        }

        // ═══════════════════════════════════════════════════════
        // SUBJECTS
        // ═══════════════════════════════════════════════════════

        // GET api/Admin/subjects
        [HttpGet("subjects")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubjects()
        {
            try
            {
                var subjects = await _adminService.GetAllSubjectsAsync();
                return Ok(new { success = true, data = subjects });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching subjects" });
            }
        }


        // POST api/Admin/subjects
        [HttpPost("subjects")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var subject = await _adminService.CreateSubjectAsync(dto);
                return Ok(new { success = true, data = subject });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating subject" });
            }
        }


        // PUT api/Admin/subjects/{subjectId}
        [HttpPut("subjects/{subjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(
            int subjectId,
            [FromBody] UpdateSubjectDto dto)
        {
            try
            {
                var result = await _adminService.UpdateSubjectAsync(subjectId, dto);
                return result
                    ? Ok(new { success = true, message = "Subject updated successfully" })
                    : NotFound(new { message = "Subject not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating subject" });
            }
        }


        // DELETE api/Admin/subjects/{subjectId}
        [HttpDelete("subjects/{subjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            try
            {
                var result = await _adminService.DeleteSubjectAsync(subjectId);
                return result
                    ? Ok(new { success = true, message = "Subject deleted successfully" })
                    : NotFound(new { message = "Subject not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting subject" });
            }
        }

        // GET api/Admin/logs
        [HttpGet("logs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLogs()
        {
            try
            {
                var logs = await _adminService.GetAdminLogsAsync();
                return Ok(new { success = true, totalCount = logs.Count, data = logs });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching logs" });
            }
        }
    }
}

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using ScholaAi.DTOs.Admin;
//using ScholaAi.Models;
//using ScholaAi.Services.Base;
//using ScholaAi.Services.Admin;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;


//namespace ScholaAi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AdminController : ControllerBase
//    {
//        private readonly IAdminService _adminService;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IConfiguration _configuration;

//        public AdminController(
//            IAdminService adminService,
//            UserManager<ApplicationUser> userManager,
//            IConfiguration configuration)
//        {
//            _adminService = adminService;
//            _userManager = userManager;
//            _configuration = configuration;
//        }

//        // ═══════════════════════════════════════════════════════
//        // AUTH
//        // POST api/Admin/login
//        // ═══════════════════════════════════════════════════════
//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Find user by email
//            var user = await _userManager.FindByEmailAsync(dto.Email);
//            if (user == null)
//                return Unauthorized(new { message = "Invalid email or password" });

//            // Make sure they are actually an Admin
//            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
//            if (!isAdmin)
//                return Unauthorized(new { message = "Access denied. Admins only." });

//            // Check password
//            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
//            if (!passwordValid)
//                return Unauthorized(new { message = "Invalid email or password" });

//            // Build JWT token
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id),
//                new Claim(ClaimTypes.Email, user.Email ?? ""),
//                new Claim(ClaimTypes.Role, "Admin")
//            };

//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(_configuration["JWT:Secretkey"]));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//            var token = new JwtSecurityToken(
//                issuer: _configuration["JWT:ValidIssuer"],
//                audience: _configuration["JWT:ValidAudience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(8),
//                signingCredentials: credentials
//            );

//            return Ok(new
//            {
//                token = new JwtSecurityTokenHandler().WriteToken(token)
//            });
//        }

//        // ═══════════════════════════════════════════════════════
//        // PROFILE
//        // GET api/Admin/profile
//        // ═══════════════════════════════════════════════════════
//        [HttpGet("profile")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetProfile()
//        {
//            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(adminId))
//                return Unauthorized();

//            var profile = await _adminService.GetAdminProfileAsync(adminId);
//            return profile == null
//                ? NotFound(new { message = "Admin not found" })
//                : Ok(new { success = true, data = profile });
//        }

//        //Update Admin Profile
//        [HttpPut("profile")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateProfile([FromBody] AdminEditProfileDto dto)
//        {
//            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(adminId))
//                return Unauthorized();

//            var result = await _adminService.UpdateAdminProfileAsync(adminId, dto);
//            return result
//                ? Ok(new { success = true, message = "Profile updated successfully" })
//                : NotFound(new { message = "Admin not found" });
//        }

//        // ═══════════════════════════════════════════════════════
//        // DASHBOARD
//        // GET api/Admin/dashboard
//        // ═══════════════════════════════════════════════════════
//        [HttpGet("dashboard")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetDashboard()
//        {
//            var data = await _adminService.GetDashboardAsync();
//            return Ok(new { success = true, data });
//        }

//        // ═══════════════════════════════════════════════════════
//        // USERS
//        // ═══════════════════════════════════════════════════════

//        // GET api/Admin/users?search=&role=

//        [HttpGet("users")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetUsers(
//            [FromQuery] string? search,
//            [FromQuery] string? role,
//            [FromQuery] int page = 1,
//            [FromQuery] int pageSize = 10)
//            {
//            var users = await _adminService.GetAllUsersAsync(search, role);

//            var totalCount = users.Count;
//            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
//            var paged = users
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            return Ok(new
//            {
//                success = true,
//                totalCount = totalCount,
//                totalPages = totalPages,
//                page = page,
//                pageSize = pageSize,
//                data = paged
//            });
//        }

//        // GET api/Admin/users/{userId}
//        [HttpGet("users/{userId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetUser(string userId)
//        {
//            var user = await _adminService.GetUserDetailAsync(userId);
//            return user == null
//                ? NotFound(new { message = "User not found" })
//                : Ok(new { success = true, data = user });
//        }

//        // POST api/Admin/users
//        [HttpPost("users")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            try
//            {
//                var created = await _adminService.CreateUserAsync(dto);
//                return Ok(new { success = true, data = created });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        // PUT api/Admin/users/{userId}
//        [HttpPut("users/{userId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> EditUser(
//            string userId,
//            [FromBody] AdminEditUserDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _adminService.EditUserAsync(userId, dto);
//            return result
//                ? Ok(new { success = true, message = "User updated successfully" })
//                : NotFound(new { message = "User not found" });
//        }

//        // DELETE api/Admin/users/{userId}
//        [HttpDelete("users/{userId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> DeleteUser(string userId)
//        {
//            var result = await _adminService.DeleteUserAsync(userId);
//            return result
//                ? Ok(new { success = true, message = "User deleted successfully" })
//                : NotFound(new { message = "User not found" });
//        }

//        // PUT api/Admin/users/{userId}/role
//        [HttpPut("users/{userId}/role")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> ChangeRole(
//            string userId,
//            [FromBody] ChangeUserRoleDto dto)
//        {
//            try
//            {
//                var result = await _adminService.ChangeUserRoleAsync(userId, dto);
//                return result
//                    ? Ok(new { success = true, message = "Role updated successfully" })
//                    : NotFound(new { message = "User not found" });
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        // POST api/Admin/users/{userId}/suspend
//        [HttpPost("users/{userId}/suspend")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Suspend(
//            string userId,
//            [FromBody] SuspendUserDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _adminService.SuspendUserAsync(userId, dto);
//            return result
//                ? Ok(new { success = true, message = $"User suspended for {dto.DurationInDays} day(s)" })
//                : NotFound(new { message = "User not found" });
//        }

//        // POST api/Admin/users/{userId}/unsuspend
//        [HttpPost("users/{userId}/unsuspend")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Unsuspend(string userId)
//        {
//            var result = await _adminService.UnsuspendUserAsync(userId);
//            return result
//                ? Ok(new { success = true, message = "User unsuspended successfully" })
//                : NotFound(new { message = "User not found" });
//        }

//        // ═══════════════════════════════════════════════════════
//        // SESSIONS
//        // ═══════════════════════════════════════════════════════

//        // GET api/Admin/sessions?search=

//        [HttpGet("sessions")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetSessions(
//           [FromQuery] string? search,
//           [FromQuery] int page = 1,
//           [FromQuery] int pageSize = 10)
//        {
//            var sessions = await _adminService.GetAllSessionsAsync(search);

//            var totalCount = sessions.Count;
//            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
//            var paged = sessions
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            return Ok(new
//            {
//                success = true,
//                totalCount = totalCount,
//                totalPages = totalPages,
//                page = page,
//                pageSize = pageSize,
//                data = paged
//            });
//        }

//        // GET api/Admin/sessions/live
//        // ⚠️ IMPORTANT: this route must come BEFORE sessions/{sessionId}
//        [HttpGet("sessions/live")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetLiveSessions()
//        {
//            var sessions = await _adminService.GetLiveSessionsAsync();
//            return Ok(new { success = true, totalCount = sessions.Count, data = sessions });
//        }

//        // GET api/Admin/sessions/{sessionId}
//        [HttpGet("sessions/{sessionId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetSession(int sessionId)
//        {
//            var session = await _adminService.GetSessionDetailAsync(sessionId);
//            return session == null
//                ? NotFound(new { message = "Session not found" })
//                : Ok(new { success = true, data = session });
//        }

//        // ═══════════════════════════════════════════════════════
//        // PAYMENTS
//        // ═══════════════════════════════════════════════════════

//        // GET api/Admin/payments/export
//        // ⚠️ IMPORTANT: this route must come BEFORE payments/{transactionId}
//        [HttpGet("payments/export")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> ExportPayments()
//        {
//            var csvBytes = await _adminService.ExportPaymentsCsvAsync();
//            return File(csvBytes, "text/csv",
//                $"payments_{DateTime.UtcNow:yyyyMMdd}.csv");
//        }

//        // GET api/Admin/payments?search=

//        [HttpGet("payments")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetPayments(
//             [FromQuery] string? search,
//             [FromQuery] int page = 1,
//             [FromQuery] int pageSize = 10)
//        {
//            var payments = await _adminService.GetAllPaymentsAsync(search);

//            var totalCount = payments.Count;
//            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
//            var paged = payments
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .ToList();

//            return Ok(new
//            {
//                success = true,
//                totalCount = totalCount,
//                totalPages = totalPages,
//                page = page,
//                pageSize = pageSize,
//                data = paged
//            });
//        }

//        // GET api/Admin/payments/{transactionId}
//        [HttpGet("payments/{transactionId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetPayment(int transactionId)
//        {
//            var payment = await _adminService.GetPaymentDetailAsync(transactionId);
//            return payment == null
//                ? NotFound(new { message = "Payment not found" })
//                : Ok(new { success = true, data = payment });
//        }

//        // ═══════════════════════════════════════════════════════
//        // SUBJECTS
//        // ═══════════════════════════════════════════════════════

//        // GET api/Admin/subjects
//        [HttpGet("subjects")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetSubjects()
//        {
//            var subjects = await _adminService.GetAllSubjectsAsync();
//            return Ok(new { success = true, data = subjects });
//        }

//        // POST api/Admin/subjects
//        [HttpPost("subjects")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var subject = await _adminService.CreateSubjectAsync(dto);
//            return Ok(new { success = true, data = subject });
//        }

//        // PUT api/Admin/subjects/{subjectId}
//        [HttpPut("subjects/{subjectId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateSubject(
//            int subjectId,
//            [FromBody] UpdateSubjectDto dto)
//        {
//            var result = await _adminService.UpdateSubjectAsync(subjectId, dto);
//            return result
//                ? Ok(new { success = true, message = "Subject updated successfully" })
//                : NotFound(new { message = "Subject not found" });
//        }

//        // DELETE api/Admin/subjects/{subjectId}
//        [HttpDelete("subjects/{subjectId}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> DeleteSubject(int subjectId)
//        {
//            var result = await _adminService.DeleteSubjectAsync(subjectId);
//            return result
//                ? Ok(new { success = true, message = "Subject deleted successfully" })
//                : NotFound(new { message = "Subject not found" });
//        }
//    }
//}
