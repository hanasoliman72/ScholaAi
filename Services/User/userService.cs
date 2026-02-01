using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teatcher;
using ScholaAi.DTOs.User;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.User
{
    public class userService :IUserService 
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly UserManager<applicationUser> _userManager;
        public userService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,IAvailabilityRepository availabilityRepository,
            IEmailService emailService, IHttpContextAccessor httpContextAccessor,
                UserManager<applicationUser> userManager)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _availabilityRepository = availabilityRepository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<studentRegisterDto> registerStudent(studentRegisterDto nUser)
        {
            
            var newUser = new user
            {
                userName = nUser.userName,
                email = nUser.email,
                firstName = nUser.firstName,
                lastName = nUser.lastName,
                phone = nUser.phone,
                description = nUser.description,
                gender = nUser.gender,
              
                profilePhotoURL = nUser.profilePhotoURL,
                applicationUserId = nUser.id,
                userType = ScholaAi.Models.Type.Student

                //passwordHash = null // Password managed by Identity, not stored in this table
            };

            await _userRepository.addAsync(newUser);

            var student = new student
            {
                userId =newUser.userId,
                grade = nUser.grade,
            };
            await _studentRepository.addAsync(student);

            if (nUser.availability != null && nUser.availability.Count > 0)
            {
                var availabilityEntities = nUser.availability.Select(a => new availability
                {
                    Day = a.Day,
                    TimeSlot = a.TimeSlot,
                    userId = newUser.userId
                }).ToList();

                await _availabilityRepository.addRangeAsync(availabilityEntities);
            }

            nUser.userId = newUser.userId;
            return nUser;
        }

        public async Task<teacherRegisterDto> registerTeacher(teacherRegisterDto nUser)
        {
            //if (nUser == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            //var existingUser = await _userRepository.getByEmailAsync(nUser.email);
            //if (existingUser != null)
            //{
            //    throw new Exception("Email already exists");
            //}

            var newUser = new user
            {
                userName = nUser.userName,
                email = nUser.email,
                firstName = nUser.firstName,
                lastName = nUser.lastName,
                phone = nUser.phone,
                description = nUser.description,
                gender = nUser.gender,
                
                // Don't store passwordHash here - Identity handles it in applicationUser
                // The applicationUserId links to Identity user
                profilePhotoURL = nUser.profilePhotoURL,
                applicationUserId = nUser.id,
                userType = ScholaAi.Models.Type.Teacher
                //passwordHash = null // Password managed by Identity, not stored in this table
            };
            await _userRepository.addAsync(newUser);
            var teacher = new teacher
            {
                userId = newUser.userId,
                certificate = nUser.certificate,
                college = nUser.college,
                teachingExperience = nUser.teachingExperience,
                subjectName = nUser.subjectName

            };
            await _teacherRepository.addAsync(teacher);

            if (nUser.availability != null && nUser.availability.Count > 0)
            {
                var availabilityEntities = nUser.availability.Select(a => new availability
                {
                    Day = a.Day,
                    TimeSlot = a.TimeSlot,
                    userId = newUser.userId
                }).ToList();

                await _availabilityRepository.addRangeAsync(availabilityEntities);
            }
            nUser.userId = newUser.userId;

            return nUser;
        }

        public async Task<user> GetUserByApplicationUserId(string appUserId)
        {
            return await _userRepository.getUserByApplicationUserId(appUserId);
        }

public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
{
    var userEntity = await _userRepository.getByEmailAsync(email);
    if (userEntity == null) return false;

    var identityUser = await _userManager.FindByIdAsync(userEntity.applicationUserId);
    if (identityUser == null) return false;

    var result = await _userRepository.resetPasswordAsync(identityUser, token, newPassword);
    return result.Succeeded;
}
        // for password 

        public async Task<bool> SendForgotPasswordEmailAsync(string email)
        {
            var user = await _userRepository.getByEmailAsync(email);
            if (user == null) return false;

            var appUser = await _userManager.FindByIdAsync(user.applicationUserId);
            if (appUser == null) return false;

            var token = await _userRepository.generatePasswordResetTokenAsync(appUser);
            var resetLink = $"https://yourfrontend.com/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(email, "Reset Password", $"Click here to reset your password: {resetLink}");
            return true;
        }

        // إعادة تعيين الباسورد باستخدام التوكن
        //public async Task<bool> ResetPasswordAsync(resetPasswordDto dto)
        //{
        //    var user = await _userRepository.getByEmailAsync(dto.Email);
        //    if (user == null) return false;

        //    var appUser = await _userManager.FindByIdAsync(user.applicationUserId);
        //    if (appUser == null) return false;

        //    var result = await _userRepository.resetPasswordAsync(appUser, dto.Token, dto.NewPassword);
        //    return result.Succeeded;
        //}
        public async Task<IdentityResult> ResetPasswordAsync(resetPasswordDto dto)
        {
            var user = await _userRepository.getByEmailAsync(dto.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var appUser = await _userManager.FindByIdAsync(user.applicationUserId);
            if (appUser == null) return IdentityResult.Failed(new IdentityError { Description = "Identity user not found" });

            var result = await _userRepository.resetPasswordAsync(appUser, dto.Token, dto.NewPassword);
            return result; 
        }

        // تغيير الباسورد بعد تسجيل الدخول
        public async Task<bool> ChangePasswordAsync(string applicationUserId, changePasswordDto dto)
        {
            var appUser = await _userManager.FindByIdAsync(applicationUserId);
            if (appUser == null) return false;

            var result = await _userManager.ChangePasswordAsync(appUser, dto.currentPassword, dto.newPassword);
            return result.Succeeded;
        }

    }
}
