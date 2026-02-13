using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IUserService
    {
        //Task<bool> ChangePasswordAsync(int userId, changePasswordDto dto);
        // Task<string?> UploadProfilePhotoAsync(int userId, IFormFile file);
        Task<StudentRegisterDto> RegisterStudent(StudentRegisterDto nUser);
        Task<TeacherRegisterDto> RegisterTeacher(TeacherRegisterDto nUser);
        Task<ApplicationUser> GetUserByApplicationUserId(string appUserId);
        Task<bool> SendForgotPasswordEmailAsync(string email);
 

        Task<IdentityResult> ResetPasswordAsync(resetPasswordDto dto);
        Task<bool> ChangePasswordAsync(string applicationUserId, changePasswordDto dto);

    }
}
