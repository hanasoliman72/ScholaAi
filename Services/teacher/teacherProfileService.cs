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
        // Get Teacher Profile By ID
        // ===============================
        public async Task<teacherProfileDto?> GetTeacherProfileAsync(int teacherId)
        {
            var teacher = await _teacherRepository.getByIdWithUserAsync(teacherId);

            if (teacher == null || teacher.user == null)
                return null;

            return new teacherProfileDto
            {
                userName = teacher.user.userName,
                email = teacher.user.email,
                firstName = teacher.user.firstName,
                lastName = teacher.user.lastName,
                description = teacher.user.description,
                profilePhotoURL = teacher.user.profilePhotoURL,
                college = teacher.college,
                teachingExperience = teacher.teachingExperience
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
                .Where(t => t.user != null)
                .Select(t => new teacherSearchResultDto
                {
                    userName = t.user.userName,
                    subject = t.subject.name,
                    college = t.college,
                    teachingExperience = t.teachingExperience,
                    profilePhotoURL = t.user.profilePhotoURL
                })
                .ToList();
        }
 
    }
}
