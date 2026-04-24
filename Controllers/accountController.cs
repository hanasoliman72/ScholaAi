using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScholaAi.DTOs;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services;
using ScholaAi.Services.Base;
using ScholaAi.Services.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScholaAi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class accountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly IPasswordService _passwordService;
        public accountController(IUserService userService, UserManager<ApplicationUser> userManager,IConfiguration configuration )
        {
            _userService = userService;
            _userManager = userManager;
            _configuration = configuration;
        }
        //private readonly userRegisterService userRegisterService;
        [HttpPost("register/Student")]


        public async Task<IActionResult> registerStudent([FromBody] StudentRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already registered." });

            await _userService.RegisterStudent(userDto);

            return Ok("Registered Successfully");
        }

        [HttpPost("register/Teacher")]
        public async Task<IActionResult> registerTeacher([FromBody] TeacherRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already registered." });

            await _userService.RegisterTeacher(userDto);

            return Ok("You Registered Successfully");

        }

        //    [HttpPost("login")]
        //    public async Task<IActionResult> Login([FromBody] loginDto userDto)
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var identityUser = await _userManager.FindByEmailAsync(userDto.email);
        //        if (identityUser == null)
        //            return Unauthorized("Invalid email or password");

        //        var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, userDto.password);
        //        if (!isPasswordValid)
        //            return Unauthorized("Invalid email or password");

        //        // 🔑 Claims
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier, identityUser.Id), // ✅ CORRECT
        //    new Claim(ClaimTypes.Email, identityUser.Email ?? string.Empty),
        //    new Claim("UserType", identityUser.UserType.ToString())
        //};

        //        // ✅ Roles
        //        var roles = await _userManager.GetRolesAsync(identityUser);
        //        foreach (var role in roles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, role));
        //        }

        //        // 🔐 JWT
        //        var secretKey = _configuration["JWT:Secretkey"];
        //        var issuer = _configuration["JWT:ValidIssuer"];
        //        var audience = _configuration["JWT:ValidAudience"];

        //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: issuer,
        //            audience: audience,
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddDays(365),
        //            signingCredentials: credentials
        //        );

        //        return Ok(new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token)
        //        });
        //    }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identityUser = await _userManager.FindByEmailAsync(userDto.email);
            if (identityUser == null)
                return Unauthorized("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, userDto.password);
            if (!isPasswordValid)
                return Unauthorized("Invalid email or password");

            // ✅ Suspension check
            if (identityUser.IsSuspended)
            {
                if (identityUser.SuspendedUntil.HasValue && identityUser.SuspendedUntil.Value > DateTime.UtcNow)
                {
                    return Unauthorized(new
                    {
                        message = $"Your account is suspended until {identityUser.SuspendedUntil.Value:yyyy-MM-dd}"
                    });
                }
                else
                {
                    // Suspension expired, auto-unsuspend
                    identityUser.IsSuspended = false;
                    identityUser.SuspendedUntil = null;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            // 🔑 Claims
            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim(ClaimTypes.Email, identityUser.Email ?? string.Empty),
                 new Claim("UserType", identityUser.UserType.ToString())
             };

            // ✅ Roles
            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 🔐 JWT
            var secretKey = _configuration["JWT:Secretkey"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(365),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }



        // ========================
        // Forgot Password
        // ========================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _userService.SendForgotPasswordEmailAsync(dto.Email);
            if (!result) return NotFound("User not found.");
            return Ok("Reset password email sent.");
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] resetPasswordDto dto)
        {
            var result = await _userService.ResetPasswordAsync(dto); // دلوقتي IdentityResult
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine(errors);
                return BadRequest(errors);
            }

            return Ok("Password has been reset successfully.");
        }
    }
}
