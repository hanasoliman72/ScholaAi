using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.User;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.Teacher
{
    public class teacherProfileService : ITeacherProfileService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserRepository _userRepository;


        public teacherProfileService(
            IUserRepository userRepository,
            ITeacherRepository teacherRepository,
            UserManager<ApplicationUser> userManager,
            IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _teacherRepository = teacherRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
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

        public async Task<bool> ChangePasswordAsync(string userId, changePasswordDto dto)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return false;

            var identityUser = await _userManager.FindByIdAsync(user.Id);
            if (identityUser == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(
                identityUser,
                dto.currentPassword,
                dto.newPassword
            );

            return result.Succeeded;
        }

    }
}
