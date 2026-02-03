using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScholaAi.DTOs;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
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
        private readonly UserManager<applicationUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly IPasswordService _passwordService;
        public accountController(IUserService userService, UserManager<applicationUser> userManager,IConfiguration configuration )
        {
            _userService = userService;
            _userManager = userManager;
            _configuration = configuration;
        }
        //private readonly userRegisterService userRegisterService;
        [HttpPost("register/student")]

        //public async Task <IActionResult> registerStudent(studentRegisterDto userDto)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        //applicationUser user = new applicationUser();
        //        //user.UserName = userDto.userName;
        //        //user.Email = userDto.email;
        //        //user.PhoneNumber = userDto.phone;


        //        ////user.PasswordHash = userDto.passwordHash;
        //        //IdentityResult result =await _userManager.CreateAsync(user,userDto.passwordHash);
        //        //if (result.Succeeded)
        //        //{
        //        //    userDto.id =user.Id;
        //        //   await _userRegisterService.registerStudent(userDto);

        //        //    return Ok("You Registerd Succefully");
        //        applicationUser identityUser = new applicationUser
        //        {
        //            UserName = userDto.userName,
        //            Email = userDto.email,
        //            PhoneNumber = userDto.phone
        //        };

        //        var result = await _userManager.CreateAsync(identityUser, userDto.Password);

        //        if (!result.Succeeded)
        //            return BadRequest(result.Errors);

        //        // مهم جدًا
        //        userDto.id = identityUser.Id;

        //        await _userRegisterService.registerStudent(userDto);

        //        return Ok("Registered Successfully");

        //    }
        //    return BadRequest(result.Errors);


        //    }
        //    return BadRequest(ModelState);

        //}

        public async Task<IActionResult> registerStudent(studentRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(userDto.email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already registered." });

            applicationUser identityUser = new applicationUser
            {
                UserName = userDto.userName,
                Email = userDto.email,
                PhoneNumber = userDto.phone
            };

            var result = await _userManager.CreateAsync(identityUser, userDto.Password);
 
            if (!result.Succeeded)
            // return BadRequest(result.Errors);
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Registration failed", errors });
            }
            await _userManager.AddToRoleAsync(identityUser, "Student");
            userDto.id = identityUser.Id;

            await _userService.registerStudent(userDto);

            return Ok("Registered Successfully");
        }

        [HttpPost("register/teacher")]
        public async Task<IActionResult> registerTeacher(teacherRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(userDto.email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already registered." });

                applicationUser user = new applicationUser();
                user.UserName = userDto.userName;
                user.Email = userDto.email;
                user.PhoneNumber = userDto.phone;

                var result = await _userManager.CreateAsync(user, userDto.Password);
              
                if (result.Succeeded)
                { 
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    userDto.id = user.Id;
                    await _userService.registerTeacher(userDto);
                   
                    return Ok("You Registered Successfully");
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Registration failed", errors });
                }
        }
        //[HttpPost("login")]
        //public async Task<IActionResult> login(loginDto userDto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        applicationUser user = await _userManager.FindByEmailAsync(userDto.email);
        //        //var user = await _userRegisterService.getUserByApplicationUserId(user.Id);

        //        if (user != null)
        //        {
        //            bool found = await _userManager.CheckPasswordAsync(user, userDto.password);
        //            if (found)
        //            {
        //                //claims tokens 
        //                var claims = new List<Claim>()
        //                {
        //                    new Claim(ClaimTypes.Email, user.Email ?? ""),
        //                    new Claim(ClaimTypes.NameIdentifier, user.Id),

        //                };
        //                var roles = await _userManager.GetRolesAsync(user);
        //                foreach (var role in roles)
        //                {
        //                    claims.Add(new Claim(ClaimTypes.Role, role));
        //                }
        //                claims.Add(new Claim("UserType", roles.FirstOrDefault() ?? "User"));

        //                var secretKey = _configuration["JWT:Secretkey"];
        //                var validIssuer = _configuration["JWT:ValidIssuer"];
        //                var validAudience = _configuration["JWT:ValidAudience"];

        //                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(validIssuer) || string.IsNullOrEmpty(validAudience))
        //                {
        //                    return StatusCode(500, "JWT configuration is missing");
        //                }

        //                SecurityKey securityKey =
        //                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //                SigningCredentials signingCredentials =
        //                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //                JwtSecurityToken token = new JwtSecurityToken(
        //                    issuer: validIssuer,
        //                    audience: validAudience,
        //                    claims: claims,
        //                    expires: DateTime.Now.AddDays(365),
        //                    signingCredentials: signingCredentials

        //                    );
        //                return Ok(
        //                    new
        //                    {
        //                        token = new JwtSecurityTokenHandler().WriteToken(token)
        //                    }
        //                    );
        //            }

        //        }
        //        return Unauthorized();
        //    }
        //    return Unauthorized();
        //}
        [HttpPost("login")]
        public async Task<IActionResult> login(loginDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identityUser = await _userManager.FindByEmailAsync(userDto.email);
            if (identityUser == null)
                return Unauthorized();

            var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, userDto.password);
            if (!isPasswordValid)
                return Unauthorized();

            var dbUser = await _userService.GetUserByApplicationUserId(identityUser.Id);
            if (dbUser == null)
                return Unauthorized();

            // ✅ Claims
            var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, dbUser.userId.ToString()), // هنا استخدمنا userId
        new Claim(ClaimTypes.Email, identityUser.Email ?? ""),
        new Claim("UserType", dbUser.userType.ToString())
    };

            var roles = await _userManager.GetRolesAsync(identityUser);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // ✅ JWT
            var secretKey = _configuration["JWT:Secretkey"];
            var validIssuer = _configuration["JWT:ValidIssuer"];
            var validAudience = _configuration["JWT:ValidAudience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(365),
                signingCredentials: signingCredentials
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

        // ========================
        // Reset Password
        // ========================
        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] resetPasswordDto dto)
        //{
        //    var result = await _userService.ResetPasswordAsync(dto);
        //    if (!result)
        //    {
        //        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //        Console.WriteLine(errors); // هنا هتعرف السبب
        //    }
        //    //return BadRequest("Failed to reset password.");
        //    return Ok("Password has been reset successfully.");
        //}

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] resetPasswordDto dto)
        {
            var result = await _userService.ResetPasswordAsync(dto); // دلوقتي IdentityResult
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine(errors); // هتعرف السبب
                return BadRequest(errors);
            }

            return Ok("Password has been reset successfully.");
        }
    }
}
