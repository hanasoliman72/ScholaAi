using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Student;
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
        public async Task<string?> uploadProfilePhotoAsync(string userId, IFormFile file)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return null;

            var photoUrl = await _fileUploadService.UploadFileAsync(file, "profile-photos");
            if (photoUrl == null)
                return null;

            user.ProfilePhotoURL = photoUrl;
            await _userRepository.updateAsync(user);

            return photoUrl;
        }
        public async Task<(bool success, string message)> UpdateTeacherProfileAsync(string userId,
            updateTeacherProfileDto dto)
        {
            var teacher = await _teacherRepository.getByIdAsync(userId);

            if (teacher == null || teacher.ApplicationUser == null)
                return (false, "Teacher profile not found.");

            var user = teacher.ApplicationUser;

            // username
            if (!string.IsNullOrWhiteSpace(dto.userName))
            {
                var userExists = await _userRepository.getUserByUserNameAsync(dto.userName);
                if (userExists != null && userExists.Id != userId)
                    return (false, "Username is already taken.");

                user.UserName = dto.userName;
            }

            // basic user info
            if (!string.IsNullOrWhiteSpace(dto.firstName))
                user.FirstName = dto.firstName;

            if (!string.IsNullOrWhiteSpace(dto.lastName))
                user.LastName = dto.lastName;

            if (!string.IsNullOrWhiteSpace(dto.phone))
                user.PhoneNumber = dto.phone;

            if (!string.IsNullOrWhiteSpace(dto.description))
                user.Description = dto.description;

            // teacher specific info
            if (!string.IsNullOrWhiteSpace(dto.college))
                teacher.College = dto.college;

            if (!string.IsNullOrWhiteSpace(dto.certificate))
                teacher.Certificate = dto.certificate;

            if (!string.IsNullOrWhiteSpace(dto.teachingExperience))
                teacher.TeachingExperience = dto.teachingExperience;

            await _userRepository.updateAsync(user);
            await _teacherRepository.updateAsync(teacher);

            return (true, "Teacher profile updated successfully");
        }

    }
}
