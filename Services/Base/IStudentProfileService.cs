using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;

namespace ScholaAi.Services
{
    public interface IStudentProfileService
    {
        Task<studentProfileDto?> getStudentProfileAsync(int userId);
        Task<bool> updateStudentProfileAsync(int userId, updateStudentProfileDto dto);
        Task<bool> changePasswordAsync(int userId, changePasswordDto dto);
        Task<string?> uploadProfilePhotoAsync(int userId, IFormFile file);
    }
}
