using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Admin;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;
using System.Text;
using static Azure.Core.HttpHeader;


namespace ScholaAi.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBcontext _context;

        public AdminService(
            IAdminRepository adminRepo,
            UserManager<ApplicationUser> userManager,
            DBcontext context)
        {
            _adminRepo = adminRepo;
            _userManager = userManager;
            _context = context;
        }

        // ═══════════════════════════════════════════════════════
        // DASHBOARD
        // ═══════════════════════════════════════════════════════
        public async Task<AdminDashboardDto> GetDashboardAsync()
            => await _adminRepo.GetDashboardStatsAsync();

        // ═══════════════════════════════════════════════════════
        // ADMIN LOGS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminLogDto>> GetAdminLogsAsync()
        {
            return await _context.AdminLogs
                .Include(l => l.Admin)
                .Include(l => l.TargetUser)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new AdminLogDto
                {
                    LogId = l.LogId,
                    AdminName = l.Admin.FirstName + " " + l.Admin.LastName,
                    TargetUserName = l.TargetUser != null
                                     ? l.TargetUser.FirstName + " " + l.TargetUser.LastName
                                     : null,
                    Details = l.Details,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }



        // ═══════════════════════════════════════════════════════
        // USERS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminUserListDto>> GetAllUsersAsync(string? search, string? role)
        {
            var users = await _adminRepo.GetAllUsersAsync(search, role);
            var result = new List<AdminUserListDto>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var userRole = roles.FirstOrDefault() ?? "None";
                result.Add(MapToListDto(u, userRole));
            }

            return result;
        }

        public async Task<AdminUserDetailDto?> GetUserDetailAsync(string userId)
        {
            var user = await _adminRepo.GetUserDetailAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "None";

            return MapToDetailDto(user, userRole);
        }

        public async Task<AdminUserDetailDto> CreateUserAsync(AdminCreateUserDto dto)
        {
            // Validate role
            var validRoles = new[] { "Student", "Teacher", "Admin" };
            if (!validRoles.Contains(dto.Role))
                throw new ArgumentException("Invalid role. Must be Student, Teacher, or Admin");

            // Create the Identity user
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Gender = Enum.Parse<Gender>(dto.Gender, ignoreCase: true),
                UserType = Enum.Parse<UserType>(dto.Role, ignoreCase: true)
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(user, dto.Role);

            // If Student → create Student record
            if (dto.Role == "Student")
            {
                if (!dto.Grade.HasValue)
                    throw new ArgumentException("Grade is required for Student");

              
                _context.Students.Add(new ScholaAi.Models.Student
                {
                    ApplicationUserId = user.Id,
                    Grade = dto.Grade.Value
                });


                await _context.SaveChangesAsync();
            }

            // If Teacher → create Teacher record
            if (dto.Role == "Teacher")
            {
                if (!dto.SubjectId.HasValue)
                    throw new ArgumentException("SubjectId is required for Teacher");

               
                _context.Teachers.Add(new ScholaAi.Models.Teacher
                {
                    ApplicationUserId = user.Id,
                    College = dto.College ?? "",
                    Certificate = dto.Certificate ?? "",
                    TeachingExperience = dto.TeachingExperience ?? "",
                    SubjectId = dto.SubjectId.Value
                });

                await _context.SaveChangesAsync();
            }

            return MapToDetailDto(user, dto.Role);
        }

        public async Task<bool> EditUserAsync(string userId, AdminEditUserDto dto)
        {
            var user = await _adminRepo.GetUserDetailAsync(userId);
            if (user == null) return false;

            // Update basic user fields
            if (!string.IsNullOrWhiteSpace(dto.UserName)) user.UserName = dto.UserName;
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.Description)) user.Description = dto.Description;

            await _userManager.UpdateAsync(user);

            // Update Student-specific fields
            if (user.Student != null && dto.Grade.HasValue)
            {
                user.Student.Grade = dto.Grade.Value;
                _context.Students.Update(user.Student);
                await _context.SaveChangesAsync();
            }

            // Update Teacher-specific fields
            if (user.Teacher != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.College))
                    user.Teacher.College = dto.College;

                if (!string.IsNullOrWhiteSpace(dto.Certificate))
                    user.Teacher.Certificate = dto.Certificate;

                if (!string.IsNullOrWhiteSpace(dto.TeachingExperience))
                    user.Teacher.TeachingExperience = dto.TeachingExperience;

                _context.Teachers.Update(user.Teacher);
                await _context.SaveChangesAsync();
            }

            return true;
        }


        //public async Task<bool> DeleteUserAsync(string adminId, string userId)
        //{
        //    // Log before deleting (after delete the user won't exist)
        //    await LogActionAsync(adminId, userId, "User deleted by admin");
        //    return await _adminRepo.DeleteUserAsync(userId);
        //}

        public async Task<bool> DeleteUserAsync(string adminId, string userId)
        {
            // Get user details BEFORE deleting so we can log their name
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var userName = user.UserName;
            var userEmail = user.Email;
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Unknown";

            // Now delete
            var result = await _adminRepo.DeleteUserAsync(userId);

            // Log AFTER successful deletion with full details
            if (result)
            {
                await LogActionAsync(
                    adminId,
                    null, // target user no longer exists
                    $"Deleted {userRole} account — Username: {userName}, Email: {userEmail}"
                );
            }

            return result;
        }
        //public async Task<bool> DeleteUserAsync(string userId)
        //    => await _adminRepo.DeleteUserAsync(userId);


        //public async Task<bool> ChangeUserRoleAsync(string userId, ChangeUserRoleDto dto)
        //{
        //    var validRoles = new[] { "Student", "Teacher", "Admin" };
        //    if (!validRoles.Contains(dto.NewRole))
        //        throw new ArgumentException("Invalid role. Must be Student, Teacher, or Admin");

        //    return await _adminRepo.ChangeUserRoleAsync(userId, dto.NewRole);
        //}

        public async Task<bool> ChangeUserRoleAsync(string adminId, string userId, ChangeUserRoleDto dto)
        {
            var validRoles = new[] { "Student", "Teacher", "Admin" };
            if (!validRoles.Contains(dto.NewRole))
                throw new ArgumentException("Invalid role. Must be Student, Teacher, or Admin");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var oldRoles = await _userManager.GetRolesAsync(user);
            var oldRole = oldRoles.FirstOrDefault() ?? "Unknown";

            var result = await _adminRepo.ChangeUserRoleAsync(userId, dto.NewRole);

            // Log the action
            if (result)
            {
                await LogActionAsync(
                    adminId,
                    userId,
                    $"Changed role — Username: {user.UserName}, " +
                    $"Old role: {oldRole}, " +
                    $"New role: {dto.NewRole}"
                );
            }

            return result;
        }


        //public async Task<bool> SuspendUserAsync(string userId, SuspendUserDto dto)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) return false;

        //    await _adminRepo.SuspendUserAsync(userId, dto.DurationInDays);
        //    return true;
        //}

        public async Task<bool> SuspendUserAsync(string adminId, string userId, SuspendUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _adminRepo.SuspendUserAsync(userId, dto.DurationInDays);

            // Log the action
            await LogActionAsync(
                adminId,
                userId,
                $"Suspended user — Username: {user.UserName}, " +
                $"Duration: {dto.DurationInDays} days, " +
                $"Reason: {dto.Reason ?? "No reason provided"}, " +
                $"Suspended until: {DateTime.UtcNow.AddDays(dto.DurationInDays):yyyy-MM-dd}"
            );
            return true;
        }

        //public async Task<bool> UnsuspendUserAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) return false;

        //    await _adminRepo.UnsuspendUserAsync(userId);
        //    return true;
        //}


        public async Task<bool> UnsuspendUserAsync(string adminId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _adminRepo.UnsuspendUserAsync(userId);

            // Log the action
            await LogActionAsync(
                adminId,
                userId,
                $"Unsuspended user — Username: {user.UserName}"
            );
            return true;
        }

        // ═══════════════════════════════════════════════════════
        // SESSIONS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminSessionListDto>> GetAllSessionsAsync(string? search)
            => await _adminRepo.GetAllSessionsAsync(search);

        public async Task<AdminSessionDetailDto?> GetSessionDetailAsync(int sessionId)
            => await _adminRepo.GetSessionDetailAsync(sessionId);

        public async Task<List<AdminSessionListDto>> GetLiveSessionsAsync()
            => await _adminRepo.GetLiveSessionsAsync();

        // ═══════════════════════════════════════════════════════
        // PAYMENTS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminPaymentListDto>> GetAllPaymentsAsync(string? search)
            => await _adminRepo.GetAllPaymentsAsync(search);

        public async Task<AdminPaymentListDto?> GetPaymentDetailAsync(int transactionId)
            => await _adminRepo.GetPaymentDetailAsync(transactionId);

        public async Task<byte[]> ExportPaymentsCsvAsync()
        {
            var payments = await _adminRepo.GetAllPaymentsAsync(null);

            var sb = new StringBuilder();

            // Header row
            sb.AppendLine("TransactionId,From,To,Amount,PlatformFee,SessionId,Date");

            // Data rows
            foreach (var p in payments)
            {
                sb.AppendLine(
                    $"{p.TransactionId}," +
                    $"{p.FromUserName}," +
                    $"{p.ToUserName}," +
                    $"{p.Amount}," +
                    $"{p.PlatformFee}," +
                    $"{p.SessionId}," +
                    $"{p.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        // ═══════════════════════════════════════════════════════
        // SUBJECTS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminSubjectDto>> GetAllSubjectsAsync()
            => await _adminRepo.GetAllSubjectsAsync();

        public async Task<AdminSubjectDto> CreateSubjectAsync(CreateSubjectDto dto)
        {
            var subject = await _adminRepo.CreateSubjectAsync(dto);
            return new AdminSubjectDto
            {
                SubjectId = subject.subjectId,
                Name = subject.name,
                Description = subject.description,
                TeacherCount = 0
            };
        }

        public async Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto)
            => await _adminRepo.UpdateSubjectAsync(subjectId, dto);

        public async Task<bool> DeleteSubjectAsync(int subjectId)
            => await _adminRepo.DeleteSubjectAsync(subjectId);

        // ═══════════════════════════════════════════════════════
        // ADMIN PROFILE
        // ═══════════════════════════════════════════════════════
        public async Task<AdminProfileDto?> GetAdminProfileAsync(string adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user == null) return null;

            return new AdminProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePhotoURL = user.ProfilePhotoURL
            };
        }

        // ═══════════════════════════════════════════════════════
        // PRIVATE HELPERS  (mapping methods)
        // ═══════════════════════════════════════════════════════
        private static AdminUserListDto MapToListDto(ApplicationUser u, string role)
            => new AdminUserListDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = role,
                IsSuspended = u.IsSuspended,
                SuspendedUntil = u.SuspendedUntil
            };

        private static AdminUserDetailDto MapToDetailDto(ApplicationUser u, string role)
            => new AdminUserDetailDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = role,
                PhoneNumber = u.PhoneNumber,
                Description = u.Description,
                ProfilePhotoURL = u.ProfilePhotoURL,
                Gender = u.Gender.ToString(),
                IsSuspended = u.IsSuspended,
                SuspendedUntil = u.SuspendedUntil,
                Grade = u.Student?.Grade,
                College = u.Teacher?.College,
                Certificate = u.Teacher?.Certificate,
                TeachingExperience = u.Teacher?.TeachingExperience,
                Subject = u.Teacher?.Subject?.name,
                TotalHoursTaught = u.Teacher?.TotalHoursTaught,
                AverageRating = u.Teacher?.TotalRates > 0
                                     ? u.Teacher.TotalRates : null
            };
        // ═══════════════════════════════════════════════════════
        // TEACHER VERIFICATION
        // ═══════════════════════════════════════════════════════

     
        public async Task<bool> VerifyTeacherAsync(string adminId, string teacherId, string? notes)
        {
            var teacher = await _context.Teachers
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
            if (teacher == null) return false;

            teacher.isVerified = true;
            teacher.verificationNotes = notes;
            await _context.SaveChangesAsync();

            // Log the action
            await LogActionAsync(
                adminId,
                teacherId,
                $"Verified teacher — Username: {teacher.ApplicationUser?.UserName}, " +
                $"Notes: {notes ?? "No notes"}"
            );
            return true;
        }


        //public async Task<bool> VerifyTeacherAsync(string teacherId, string? notes)
        //{
        //    var teacher = await _context.Teachers
        //        .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
        //    if (teacher == null) return false;

        //    teacher.isVerified = true;
        //    teacher.verificationNotes = notes;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<bool> UnverifyTeacherAsync(string teacherId)
        //{
        //    var teacher = await _context.Teachers
        //        .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
        //    if (teacher == null) return false;

        //    teacher.isVerified = false;
        //    teacher.verificationNotes = null;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

       
        public async Task<bool> UnverifyTeacherAsync(string adminId, string teacherId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == teacherId);
            if (teacher == null) return false;

            teacher.isVerified = false;
            teacher.verificationNotes = null;
            await _context.SaveChangesAsync();

            await LogActionAsync(
                adminId,
                teacherId,
                $"Unverified teacher — Username: {teacher.ApplicationUser?.UserName}"
            );
            return true;
        }



        // ═══════════════════════════════════════════════════════
        // RATINGS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminRatingDto>> GetAllRatingsAsync()
        {
            return await _context.Ratings
                .Include(r => r.Teacher)
                .Include(r => r.Student)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminRatingDto
                {
                    RatingId = r.RatingId,
                    TeacherName = r.Teacher.FirstName + " " + r.Teacher.LastName,
                    StudentName = r.Student != null
                                  ? r.Student.FirstName + " " + r.Student.LastName
                                  : null,
                    RatingValue = r.RatingValue,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════
        // ADMIN PROFILE UPDATE & CHANGE PASSWORD
        // ═══════════════════════════════════════════════════════
        public async Task<bool> UpdateAdminProfileAsync(string adminId, AdminEditProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangeAdminPasswordAsync(string adminId, AdminChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            return result.Succeeded;
        }

        // ═══════════════════════════════════════════════════════
        // ADMIN LOGS HELPER
        // ═══════════════════════════════════════════════════════
        private async Task LogActionAsync(string adminId, string? targetUserId, string details)
        {
            try
            {
                _context.AdminLogs.Add(new AdminLogs
                {
                    AdminId = adminId,
                    TargetUserId = targetUserId,
                    Details = details,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Logging should never break the main action
            }
        }

    }


}