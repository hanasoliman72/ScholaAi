using Microsoft.AspNetCore.Identity;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Services
{
    public class studentProfileService : IStudentProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPasswordHasher<user> _passwordHasher;
        private readonly IFileUploadService _fileUploadService;

        public studentProfileService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            IPasswordHasher<user> passwordHasher,
            IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _passwordHasher = passwordHasher;
            _fileUploadService = fileUploadService;
        }

        public async Task<studentProfileDto?> getStudentProfileAsync(int userId)
        {
            var student = await _studentRepository.getByIdAsync(userId);
            if (student == null || student.user == null)
                return null;

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var sessionsThisMonth = student.sessions
                .Count(s => s.transaction != null && s.transaction.createdAt >= startOfMonth);

            var totalHours = student.sessions
                .Where(s => s.recordedSession > 0)
                .Sum(s => s.recordedSession) / 3600.0m;

            var focusScores = student.sessions
                .Where(s => s.focusScore >= 0) // ensure score exists
                .Select(s => s.focusScore)
                .ToList();

            double avgFocusScore = 0;
            //Console.WriteLine("------------------------------");
            //Console.WriteLine(focusScores.Count);
            //Console.WriteLine("------------------------------");
            if (focusScores.Count > 0)
                avgFocusScore = (double)focusScores.Average();

            return new studentProfileDto
            {
                userName = student.user.userName,
                firstName = student.user.firstName,
                lastName = student.user.lastName,
                email = student.user.email,
                phone = student.user.phone,
                description = student.user.description,
                profilePhotoURL = student.user.profilePhotoURL,
                grade = student.grade,
                totalSessions = student.sessions.Count,
                totalHours = totalHours,
                averageFocusScore = avgFocusScore,
                sessionsThisMonth = sessionsThisMonth,
                walletBalance = student.user.wallet?.balance
            };
        }

        public async Task<bool> updateStudentProfileAsync(int userId, updateStudentProfileDto dto)
        {
            var student = await _studentRepository.getByIdAsync(userId);
            if (student == null || student.user == null)
                return false;

            var user = student.user;

            if (!string.IsNullOrWhiteSpace(dto.userName))
                user.userName = dto.userName;

            if (!string.IsNullOrWhiteSpace(dto.firstName))
                user.firstName = dto.firstName;

            if (!string.IsNullOrWhiteSpace(dto.lastName))
                user.lastName = dto.lastName;

            if (!string.IsNullOrWhiteSpace(dto.phone))
                user.phone = dto.phone;

            if (!string.IsNullOrWhiteSpace(dto.description))
                user.description = dto.description;

            if (dto.grade.HasValue)
                student.grade = dto.grade.Value;

            await _userRepository.updateAsync(user);
            await _studentRepository.updateAsync(student);
            return true;
        }

        public async Task<bool> changePasswordAsync(int userId, changePasswordDto dto)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return false;

            var result = _passwordHasher.VerifyHashedPassword(user, user.passwordHash, dto.currentPassword);

            if (result == PasswordVerificationResult.Failed)
                return false;

            user.passwordHash = _passwordHasher.HashPassword(user, dto.newPassword);
            await _userRepository.updateAsync(user);
            return true;
        }

        public async Task<string?> uploadProfilePhotoAsync(int userId, IFormFile file)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return null;

            var photoUrl = await _fileUploadService.UploadFileAsync(file, "profile-photos");
            user.profilePhotoURL = photoUrl;
            await _userRepository.updateAsync(user);

            return photoUrl;
        }
    }
}
