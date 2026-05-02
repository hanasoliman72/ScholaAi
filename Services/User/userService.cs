using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class UserService : IUserService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IAvailabilityRepository availabilityRepository,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _availabilityRepository = availabilityRepository;
            _emailService = emailService;
            _userManager = userManager;
        }

        // ================= STUDENT =================
        public async Task<StudentRegisterDto> RegisterStudent(StudentRegisterDto dto) {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.Phone,
                Description = dto.Description,
                Gender = dto.Gender,
                ProfilePhotoURL = dto.ProfilePhotoURL,
                UserType = UserType.Student
            };
            //Console.WriteLine($"FirstName: '{dto.FirstName}'");

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(user, "Student");

            // إنشاء سجل الطالب
            await _studentRepository.AddAsync(new Models.Student
            {
                ApplicationUserId = user.Id,
                Grade = dto.Grade
            });

            // إضافة availability لو موجودة
            if (dto.Availability != null && dto.Availability.Any())
            {
                await _availabilityRepository.AddRangeAsync(
                    dto.Availability.Select(a => new Availability
                    {
                        ApplicationUserId = user.Id,
                        day = a.Day,
                        timeSlot = a.TimeSlot
                    }).ToList()
                );
            }

            return dto;
        }

        // ================= TEACHER =================
        public async Task<TeacherRegisterDto> RegisterTeacher(TeacherRegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.Phone,
                Description = dto.Description,
                Gender = dto.Gender,
                ProfilePhotoURL = dto.ProfilePhotoURL,
                UserType = UserType.Teacher

            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(user, "Teacher");

            // إنشاء سجل المعلم
            await _teacherRepository.AddAsync(new Models.Teacher
            {
                ApplicationUserId = user.Id,
                College = dto.College,
                Certificate = dto.Certificate,
                TeachingExperience = dto.TeachingExperience,
                SubjectId = dto.SubjectId,
                 IdNumber = dto.IdNumber
    });

            // إضافة availability لو موجودة
            if (dto.Availability != null && dto.Availability.Any())
            {
                await _availabilityRepository.AddRangeAsync(
                    dto.Availability.Select(a => new Availability
                    {
                        ApplicationUserId = user.Id,
                        day = a.Day,
                        timeSlot = a.TimeSlot
                    }).ToList()
                );
            }

            return dto;
        }

        // ================= PASSWORD =================
        public async Task<bool> SendForgotPasswordEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourfrontend.com/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(email, "Reset Password", resetLink);
            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(resetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        }

        public async Task<bool> ChangePasswordAsync(string applicationUserId, changePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(applicationUserId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, dto.currentPassword, dto.newPassword);
            return result.Succeeded;
        }

        public async Task<ApplicationUser> GetUserByApplicationUserId(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
