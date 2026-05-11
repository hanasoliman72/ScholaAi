using Microsoft.AspNetCore.Http;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;

namespace ScholaAi.Services.Teacher
{
    public interface ITeacherProfileService
    {
        // Existing methods
        Task<teacherProfileDto?> GetTeacherProfileAsync(string teacherId);
        Task<List<teacherSearchResultDto>> SearchTeachersAsync(
            string? name,
            string? subject,
            string? keyword);
        Task<bool> ChangePasswordAsync(string userId, changePasswordDto dto);
        Task<string?> uploadProfilePhotoAsync(string userId, IFormFile file);
        Task<(bool success, string message)> UpdateTeacherProfileAsync(
            string userId, updateTeacherProfileDto dto);

        // ✅ NEW - My Students
        Task<MyStudentsListResponseDto> GetMyStudentsAsync(string teacherId, string? search);
        Task<StudentProgressDto?> GetStudentProgressAsync(string teacherId, string studentId);
    }
}