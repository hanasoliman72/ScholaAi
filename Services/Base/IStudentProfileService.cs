using Microsoft.AspNetCore.Http;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;

namespace ScholaAi.Services
{
    public interface IStudentProfileService
    {
        Task<studentProfileDto?> getStudentProfileAsync(int userId);
        Task<(bool success, string message)> updateStudentProfileAsync(int userId, updateStudentProfileDto dto);
        Task<bool> changePasswordAsync(int userId, DTOs.Common.changePasswordDto dto);
        Task<string?> uploadProfilePhotoAsync(int userId, IFormFile file);
    }
}
