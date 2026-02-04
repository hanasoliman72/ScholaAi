using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;

namespace ScholaAi.Services.Teacher
{
    public interface ITeacherProfileService
    {
        Task<teacherProfileDto?> GetTeacherProfileAsync(int teacherId);
        Task<List<teacherSearchResultDto>> SearchTeachersAsync(
           string? name,
           string? subject,
           string? keyword);
    }
}
