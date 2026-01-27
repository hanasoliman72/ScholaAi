using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using ScholaAi.DTOs.Common;
using ScholaAi.DTOs.Student;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services
{
    public class studentProfileService : IStudentProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<applicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public studentProfileService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            UserManager<applicationUser> userManager,
            IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
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

            var paymentHistory = getPaymentHistory(student);

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
                walletBalance = student.user.wallet?.balance,
                paymentHistory = paymentHistory
            };
        }


        // Helper method to get paymentHistory
        private List<PaymentHistoryItemDto> getPaymentHistory(student student)
        {
            if(student.user?.wallet == null) return new List<PaymentHistoryItemDto>();

            // Get all transactions where this user paid form his wallet
            var transactions = student.user.wallet.transactionsFrom
                                .OrderByDescending(t => t.createdAt)
                                .Take(3) // Get last 3 transactions
                                .Select(t => new PaymentHistoryItemDto
                                {
                                    transactionId = t.transactionId,
                                    amount = t.amount,
                                    date = t.createdAt
                                })
                                .ToList();
            return transactions;
        }

        public async Task<(bool success, string message)> updateStudentProfileAsync(int userId, updateStudentProfileDto dto)

        {
            var student = await _studentRepository.getByIdAsync(userId);
            if (student == null || student.user == null)
                return (false, "Student profile not found.");

            var user = student.user;


            if (!string.IsNullOrWhiteSpace(dto.userName))
            {
                var userExists = await _userRepository.getUserByUserNameAsync(dto.userName);
                if (userExists != null)
                    return (false, "Username is already taken.");
                user.userName = dto.userName;
            }
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
            return (true, "Profile updated successfully");
        }

        //public async Task<bool> changePasswordAsync(int userId, DTOs.Common.changePasswordDto dto)
        //{
        //    var user = await _userRepository.getByIdAsync(userId);
        //    if (user == null || string.IsNullOrEmpty(user.applicationUserId))
        //        return false;

        //    // Find the Identity user (applicationUser) using applicationUserId
        //    var identityUser = await _userManager.FindByIdAsync(user.applicationUserId);
        //    if (identityUser == null)
        //        return false;

        //    // Verify current password using Identity
        //    bool passwordValid = await _userManager.CheckPasswordAsync(identityUser, dto.currentPassword);
        //    if (!passwordValid)
        //        return false;

        //    // Change password using Identity
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
        //    var result = await _userManager.ResetPasswordAsync(identityUser, token, dto.newPassword);

        //    return result.Succeeded;
        //}
        public async Task<bool> changePasswordAsync(int userId, DTOs.Common.changePasswordDto dto)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.applicationUserId))
                return false;

            // Find the Identity user (applicationUser) using applicationUserId
            var identityUser = await _userManager.FindByIdAsync(user.applicationUserId);
            if (identityUser == null)
                return false;

            // Verify current password using Identity
            bool passwordValid = await _userManager.CheckPasswordAsync(identityUser, dto.currentPassword);
            if (!passwordValid)
                return false;

            // Change password using Identity
            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
            var result = await _userManager.ResetPasswordAsync(identityUser, token, dto.newPassword);

            return result.Succeeded;
        }


        public async Task<string?> uploadProfilePhotoAsync(int userId, IFormFile file)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null)
                return null;

            var photoUrl = await _fileUploadService.UploadFileAsync(file, "profile-photos");
            if (photoUrl == null)
                return null;

            user.profilePhotoURL = photoUrl;
            await _userRepository.updateAsync(user);

            return photoUrl;
        }


    }
}
