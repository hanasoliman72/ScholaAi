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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public studentProfileService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            UserManager<ApplicationUser> userManager,
            IFileUploadService fileUploadService)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public async Task<studentProfileDto?> getStudentProfileAsync(string userId)
        {
            var student = await _studentRepository.GetByIdAsync(userId);
            if (student == null || student.ApplicationUser == null)
                return null;

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var sessionsThisMonth = student.Sessions
                .Count(s => s.Transaction != null && s.Transaction.CreatedAt >= startOfMonth);

            var totalHours = student.Sessions
                .Where(s => s.RecordingDuration > 0)
                .Sum(s => s.RecordingDuration) / 3600.0m;

            var focusScores = student.Sessions
                .Where(s => s.FocusScore >= 0) // ensure score exists
                .Select(s => s.FocusScore)
                .ToList();

            double avgFocusScore = 0;
            //Console.WriteLine("------------------------------");
            //Console.WriteLine(focusScores.Count);
            //Console.WriteLine("------------------------------");
            if (focusScores.Count > 0)
                avgFocusScore = (double)focusScores.Average();

            var paymentHistory = getPaymentHistory(student);

            var lastTopUp = student.ApplicationUser?.Wallet?.TransactionsTo?
                .Where(t => t.FromWalletId == null)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => (DateTime?)t.CreatedAt)
                .FirstOrDefault();

            return new studentProfileDto
            {
                userName = student.ApplicationUser.UserName,
                firstName = student.ApplicationUser.FirstName,
                lastName = student.ApplicationUser.LastName,
                email = student.ApplicationUser.Email,
                phone = student.ApplicationUser.PhoneNumber,
                description = student.ApplicationUser.Description,
                profilePhotoURL = student.ApplicationUser.ProfilePhotoURL,
                grade = student.Grade,
                totalSessions = student.Sessions.Count,
                totalHours = totalHours,
                averageFocusScore = avgFocusScore,
                sessionsThisMonth = sessionsThisMonth,
                walletBalance = student.ApplicationUser.Wallet?.Balance,
                lastTopUp = lastTopUp,
                paymentHistory = paymentHistory
            };
        }


        // Helper method to get paymentHistory
        private List<PaymentHistoryItemDto> getPaymentHistory(Models.Student student)
        {
            if(student.ApplicationUser?.Wallet == null) return new List<PaymentHistoryItemDto>();

            // Get all transactions where this ApplicationUser paid form his Wallet
            var transactions = student.ApplicationUser.Wallet.TransactionsFrom
                                .OrderByDescending(t => t.CreatedAt)
                                .Take(3) // Get last 3 transactions
                                .Select(t => new PaymentHistoryItemDto
                                {
                                    transactionId = t.TransactionId,
                                    amount = t.Amount,
                                    date = t.CreatedAt
                                })
                                .ToList();
            return transactions;
        }

        public async Task<(bool success, string message)> updateStudentProfileAsync(string userId, updateStudentProfileDto dto)

        {
            var student = await _studentRepository.GetByIdAsync(userId);
            if (student == null || student.ApplicationUser == null)
                return (false, "Student profile not found.");

            var user = student.ApplicationUser;


            if (!string.IsNullOrWhiteSpace(dto.userName))
            {
                var userExists = await _userRepository.getUserByUserNameAsync(dto.userName);
                if (userExists != null)
                    return (false, "Username is already taken.");
                user.UserName = dto.userName;
            }
            if (!string.IsNullOrWhiteSpace(dto.firstName))
                user.FirstName = dto.firstName;

            if (!string.IsNullOrWhiteSpace(dto.lastName))
                user.LastName = dto.lastName;

            user.PhoneNumber = string.IsNullOrWhiteSpace(dto.phone) ? null : dto.phone;
            user.Description = string.IsNullOrWhiteSpace(dto.description) ? null : dto.description;

            if (dto.grade.HasValue)
                student.Grade = dto.grade.Value;

            await _userRepository.updateAsync(user);
            await _studentRepository.updateAsync(student);
            return (true, "Profile updated successfully");
        }

        //public async Task<bool> changePasswordAsync(int userId, DTOs.Common.changePasswordDto dto)
        //{
        //    var ApplicationUser = await _userRepository.getByIdAsync(userId);
        //    if (ApplicationUser == null || string.IsNullOrEmpty(ApplicationUser.applicationUserId))
        //        return false;

        //    // Find the Identity ApplicationUser (App) using applicationUserId
        //    var identityUser = await _userManager.FindByIdAsync(ApplicationUser.applicationUserId);
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
        public async Task<bool> changePasswordAsync(string userId, DTOs.Common.changePasswordDto dto)
        {
            var user = await _userRepository.getByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Id))
                return false;

            // Find the Identity ApplicationUser (App) using applicationUserId
            var identityUser = await _userManager.FindByIdAsync(user.Id);
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


    }
}
