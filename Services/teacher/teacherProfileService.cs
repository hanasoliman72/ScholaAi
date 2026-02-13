using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Services.Teacher
{
    public class teacherProfileService : ITeacherProfileService
    {
        private readonly ITeacherRepository _teacherRepository;

        public teacherProfileService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        // ===============================
        // Get Teacher Profile By ID (string ApplicationUserId)
        // ===============================
        public async Task<teacherProfileDto?> GetTeacherProfileAsync(string teacherId)
        {
            var teacher = await _teacherRepository.getByIdWithUserAsync(teacherId);

            if (teacher == null || teacher.ApplicationUser == null)
                return null;

            return new teacherProfileDto
            {
                userName = teacher.ApplicationUser.UserName,
                email = teacher.ApplicationUser.Email,
                firstName = teacher.ApplicationUser.FirstName,
                lastName = teacher.ApplicationUser.LastName,
                description = teacher.ApplicationUser.Description,
                profilePhotoURL = teacher.ApplicationUser.ProfilePhotoURL,
                college = teacher.College,
                teachingExperience = teacher.TeachingExperience
            };
        }

        // ===============================
        // Student Search About Teachers
        // ===============================
        public async Task<List<teacherSearchResultDto>> SearchTeachersAsync(
               string? name,
               string? subject,
               string? keyword)
        {
            var teachers = await _teacherRepository
                .SearchTeachersAsync(name, subject, keyword);

            return teachers
                .Where(t => t.ApplicationUser != null)
                .Select(t => new teacherSearchResultDto
                {
                    userName = t.ApplicationUser.UserName,
                    subject = t.Subject.name,
                    college = t.College,
                    teachingExperience = t.TeachingExperience,
                    profilePhotoURL = t.ApplicationUser.ProfilePhotoURL
                })
                .ToList();
        }
 
    }
}
