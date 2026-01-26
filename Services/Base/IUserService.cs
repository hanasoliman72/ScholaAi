using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IUserService
    {
        //Task<bool> ChangePasswordAsync(int userId, changePasswordDto dto);
        // Task<string?> UploadProfilePhotoAsync(int userId, IFormFile file);
        Task<studentRegisterDto> registerStudent(studentRegisterDto nUser);
        Task<teacherRegisterDto> registerTeacher(teacherRegisterDto nUser);
        Task<user> GetUserByApplicationUserId(string appUserId);
        Task<bool> SendForgotPasswordEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        Task<IdentityResult> ResetPasswordAsync(resetPasswordDto dto);
        Task<bool> ChangePasswordAsync(string applicationUserId, changePasswordDto dto);

    }
}
