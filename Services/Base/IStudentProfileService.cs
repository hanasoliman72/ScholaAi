using Microsoft.AspNetCore.Http;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;

namespace ScholaAi.Services
{
    public interface IStudentProfileService
    {
        Task<studentProfileDto?> getStudentProfileAsync(string userId);
        Task<(bool success, string message)> updateStudentProfileAsync(string userId, updateStudentProfileDto dto);
        Task<bool> changePasswordAsync(string userId, DTOs.Common.changePasswordDto dto);
        Task<string?> uploadProfilePhotoAsync(string userId, IFormFile file);
    }
}
