using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;

namespace ScholaAi.Services.Teacher
{
    public interface ITeacherProfileService
    {
        Task<teacherProfileDto?> GetTeacherProfileAsync(string teacherId);
        Task<List<teacherSearchResultDto>> SearchTeachersAsync(
           string? name,
           string? subject,
           string? keyword);
        Task<bool> ChangePasswordAsync(string userId, changePasswordDto dto);
    }
}
