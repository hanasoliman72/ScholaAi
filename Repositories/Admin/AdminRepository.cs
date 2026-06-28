using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.DTOs.Admin;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;


namespace ScholaAi.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DBcontext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminRepository(DBcontext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ═══════════════════════════════════════════════════════
        // DASHBOARD
        // ═══════════════════════════════════════════════════════
        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;

            var totalUsers = await _context.Users.CountAsync();
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();

            var activeSessions = await _context.SessionRequests
                .CountAsync(r => r.FinalScheduledAt.HasValue
                              && r.FinalScheduledAt.Value > now
                              && r.Status == RequestStatus.Accepted);

            var monthlyRevenue = await _context.Transactions
                .Where(t => t.CreatedAt.Month == now.Month
                         && t.CreatedAt.Year == now.Year)
                .SumAsync(t => (decimal?)t.PlatformFee) ?? 0;

            var sessionsThisMonth = await _context.SessionRequests
                .CountAsync(r => r.FinalScheduledAt.HasValue
                              && r.FinalScheduledAt.Value.Month == now.Month
                              && r.FinalScheduledAt.Value.Year == now.Year);

            double avgRating = await _context.Ratings.AnyAsync()
                ? await _context.Ratings.AverageAsync(r => (double)r.RatingValue)
                : 0;

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                ActiveSessions = activeSessions,
                MonthlyRevenue = monthlyRevenue,
                AverageRating = (decimal)Math.Round(avgRating, 2),
                TotalSessionsThisMonth = sessionsThisMonth
            };
        }

        // ═══════════════════════════════════════════════════════
        // USERS
        // ═══════════════════════════════════════════════════════
        public async Task<List<ApplicationUser>> GetAllUsersAsync(string? search, string? role)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    u.Email.Contains(search) ||
                    u.UserName.Contains(search) ||
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search));

            var users = await query.ToListAsync();

            // Role filtering must go through UserManager
            if (!string.IsNullOrWhiteSpace(role))
            {
                var filtered = new List<ApplicationUser>();
                foreach (var u in users)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    if (roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                        filtered.Add(u);
                }
                return filtered;
            }

            return users;
        }

        public async Task<ApplicationUser?> GetUserDetailAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t.Subject)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        //public async Task<bool> DeleteUserAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) return false;

        //    var result = await _userManager.DeleteAsync(user);
        //    return result.Succeeded;
        //}
        //public async Task<bool> DeleteUserAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null) return false;

        //    // Delete related records first to avoid foreign key conflicts

        //    // Delete Availability records
        //    var availability = _context.Availability
        //        .Where(a => a.ApplicationUserId == userId);
        //    _context.Availability.RemoveRange(availability);

        //    // Delete Student record if exists
        //    var student = await _context.Students
        //        .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
        //    if (student != null)
        //        _context.Students.Remove(student);

        //    // Delete Teacher record if exists
        //    var teacher = await _context.Teachers
        //        .FirstOrDefaultAsync(t => t.ApplicationUserId == userId);
        //    if (teacher != null)
        //        _context.Teachers.Remove(teacher);

        //    // Delete Wallet if exists
        //    var wallet = await _context.Wallets
        //        .FirstOrDefaultAsync(w => w.ApplicationUserId == userId);
        //    if (wallet != null)
        //        _context.Wallets.Remove(wallet);

        //    // Save all deletions before deleting the user
        //    await _context.SaveChangesAsync();

        //    // Now delete the Identity user
        //    var result = await _userManager.DeleteAsync(user);
        //    return result.Succeeded;
        //}

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Collect the IDs of sessions and session-requests this user is involved in,
            // so we can delete child records that reference them.
            var userSessionIds = await _context.Sessions
                .Where(s => s.TeacherId == userId || s.StudentId == userId)
                .Select(s => s.SessionId)
                .ToListAsync();

            var userRequestIds = await _context.SessionRequests
                .Where(r => r.TeacherId == userId || r.StudentId == userId)
                .Select(r => r.RequestId)
                .ToListAsync();

            // ── Step 1: Transactions ─────────────────────────────────────────────
            // Transactions FK → Wallet (FromWalletId / ToWalletId) with NoAction,
            // and FK → Session (SessionId) with NoAction.
            // Must be deleted BEFORE Wallet and BEFORE Sessions.
            var transactions = _context.Transactions
                .Where(t => t.FromWalletId == userId
                         || t.ToWalletId == userId
                         || (t.SessionId.HasValue && userSessionIds.Contains(t.SessionId.Value)));
            _context.Transactions.RemoveRange(transactions);

            // ── Step 2: Ratings ──────────────────────────────────────────────────
            // Ratings FK → Session (SessionId) with NoAction — must go before Sessions.
            // Rating.TeacherId / Rating.StudentId are ApplicationUserId strings.
            var ratings = _context.Ratings
                .Where(r => r.TeacherId == userId
                         || r.StudentId == userId
                         || userSessionIds.Contains(r.SessionId));
            _context.Ratings.RemoveRange(ratings);

            // ── Step 3: AdminLogs that point to a SessionRequest ─────────────────
            // FK → TargetRequestId (NoAction) — must go before SessionRequests.
            var logsWithRequest = _context.AdminLogs
                .Where(l => l.TargetRequestId.HasValue && userRequestIds.Contains(l.TargetRequestId.Value));
            _context.AdminLogs.RemoveRange(logsWithRequest);

            // ── Step 4: RequestBroadcasts ────────────────────────────────────────
            // FK → TeacherId (NoAction) and FK → RequestId (Cascade, but teacher FK blocks it).
            // Must go before SessionRequests.
            var broadcasts = _context.RequestBroadcasts
                .Where(rb => rb.TeacherId == userId
                          || userRequestIds.Contains(rb.RequestId));
            _context.RequestBroadcasts.RemoveRange(broadcasts);

            // ── Step 5: Notifications linked to user sessions ────────────────────
            // FK → SessionId (NoAction) — clear session-linked notifications first.
            var sessionNotifications = _context.Notifications
                .Where(n => n.SessionId.HasValue && userSessionIds.Contains(n.SessionId.Value));
            _context.Notifications.RemoveRange(sessionNotifications);

            await _context.SaveChangesAsync(); // flush before removing Sessions

            // ── Step 6: Sessions ─────────────────────────────────────────────────
            var sessions = _context.Sessions
                .Where(s => s.TeacherId == userId || s.StudentId == userId);
            _context.Sessions.RemoveRange(sessions);

            await _context.SaveChangesAsync(); // flush before removing SessionRequests

            // ── Step 7: SessionRequests ──────────────────────────────────────────
            var sessionRequests = _context.SessionRequests
                .Where(r => r.TeacherId == userId || r.StudentId == userId);
            _context.SessionRequests.RemoveRange(sessionRequests);

            // ── Step 8: Availability ─────────────────────────────────────────────
            var availability = _context.Availability
                .Where(a => a.ApplicationUserId == userId);
            _context.Availability.RemoveRange(availability);

            // ── Step 9: Remaining Notifications (sender/receiver) ────────────────
            var notifications = _context.Notifications
                .Where(n => n.SenderId == userId || n.ReceiverId == userId);
            _context.Notifications.RemoveRange(notifications);

            // ── Step 10: Chat messages ───────────────────────────────────────────
            var messages = _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId);
            _context.ChatMessages.RemoveRange(messages);

            // ── Step 11: AdminLogs where this user is the target ─────────────────
            var logs = _context.AdminLogs
                .Where(l => l.TargetUserId == userId);
            _context.AdminLogs.RemoveRange(logs);

            // ── Step 12: Wallet ──────────────────────────────────────────────────
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.ApplicationUserId == userId);
            if (wallet != null)
                _context.Wallets.Remove(wallet);

            // ── Step 13: Teacher / Student profile rows ──────────────────────────
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.ApplicationUserId == userId);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (student != null) _context.Students.Remove(student);
            if (teacher != null) _context.Teachers.Remove(teacher);

            await _context.SaveChangesAsync();

            // ── Final: Delete the Identity user ──────────────────────────────────
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, newRole);
            return result.Succeeded;
        }

        public async Task SuspendUserAsync(string userId, int days)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.IsSuspended = true;
            user.SuspendedUntil = DateTime.UtcNow.AddDays(days);
            await _userManager.UpdateAsync(user);
        }

        public async Task UnsuspendUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.IsSuspended = false;
            user.SuspendedUntil = null;
            await _userManager.UpdateAsync(user);
        }

        // ═══════════════════════════════════════════════════════
        // SESSIONS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminSessionListDto>> GetAllSessionsAsync(string? search)
        {
            var now = DateTime.UtcNow;

            var sessions = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .ToListAsync();

            var result = sessions.Select(s => new AdminSessionListDto
            {
                SessionId = s.SessionId,
                TeacherName = (s.Teacher?.ApplicationUser?.FirstName ?? "") + " "
                            + (s.Teacher?.ApplicationUser?.LastName ?? ""),
                StudentName = (s.Student?.ApplicationUser?.FirstName ?? "") + " "
                            + (s.Student?.ApplicationUser?.LastName ?? ""),
                SubjectName = s.SessionRequest?.Subject?.name ?? "N/A",
                ScheduledAt = s.SessionRequest?.FinalScheduledAt,
                FocusScore = s.FocusScore,
                IsLive = s.SessionRequest?.FinalScheduledAt.HasValue == true
                           && s.SessionRequest.FinalScheduledAt.Value <= now
                           && s.FocusScore == null
            }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                result = result
                    .Where(s =>
                        s.TeacherName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.StudentName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.SubjectName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return result;
        }

        public async Task<AdminSessionDetailDto?> GetSessionDetailAsync(int sessionId)
        {
            var now = DateTime.UtcNow;

            var s = await _context.Sessions
                .Include(s => s.SessionRequest)
                    .ThenInclude(r => r.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.ApplicationUser)
                .Include(s => s.Student)
                    .ThenInclude(st => st.ApplicationUser)
                .Include(s => s.Transaction)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (s == null) return null;

            return new AdminSessionDetailDto
            {
                SessionId = s.SessionId,
                TeacherId = s.TeacherId,
                StudentId = s.StudentId,
                TeacherName = (s.Teacher?.ApplicationUser?.FirstName ?? "") + " "
                                       + (s.Teacher?.ApplicationUser?.LastName ?? ""),
                StudentName = (s.Student?.ApplicationUser?.FirstName ?? "") + " "
                                       + (s.Student?.ApplicationUser?.LastName ?? ""),
                SubjectName = s.SessionRequest?.Subject?.name ?? "N/A",
                ScheduledAt = s.SessionRequest?.FinalScheduledAt,
                FocusScore = s.FocusScore,
                Summary = s.Summary,
                RecordedSessionSeconds = s.RecordingDuration,
                TransactionAmount = s.Transaction?.Amount,
                IsLive = s.SessionRequest?.FinalScheduledAt.HasValue == true
                                      && s.SessionRequest.FinalScheduledAt.Value <= now
                                      && s.FocusScore == null
            };
        }

        public async Task<List<AdminSessionListDto>> GetLiveSessionsAsync()
        {
            var all = await GetAllSessionsAsync(null);
            return all.Where(s => s.IsLive).ToList();
        }

        // ═══════════════════════════════════════════════════════
        // PAYMENTS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminPaymentListDto>> GetAllPaymentsAsync(string? search)
        {
            var payments = await _context.Transactions
                .Include(t => t.FromWallet)
                    .ThenInclude(w => w.ApplicationUser)
                .Include(t => t.ToWallet)
                    .ThenInclude(w => w.ApplicationUser)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new AdminPaymentListDto
                {
                    TransactionId = t.TransactionId,
                    FromUserName = t.FromWallet != null && t.FromWallet.ApplicationUser != null ? t.FromWallet.ApplicationUser.UserName : "System/Stripe",
                    ToUserName = t.ToWallet != null && t.ToWallet.ApplicationUser != null ? t.ToWallet.ApplicationUser.UserName : "System/Stripe",
                    Amount = t.Amount,
                    PlatformFee = t.PlatformFee,
                    CreatedAt = t.CreatedAt,
                    SessionId = t.SessionId ?? 0
                })
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
                payments = payments
                    .Where(p =>
                        p.FromUserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.ToUserName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return payments;
        }

        public async Task<AdminPaymentListDto?> GetPaymentDetailAsync(int transactionId)
        {
            return await _context.Transactions
                .Include(t => t.FromWallet).ThenInclude(w => w.ApplicationUser)
                .Include(t => t.ToWallet).ThenInclude(w => w.ApplicationUser)
                .Where(t => t.TransactionId == transactionId)
                .Select(t => new AdminPaymentListDto
                {
                    TransactionId = t.TransactionId,
                    FromUserName = t.FromWallet != null && t.FromWallet.ApplicationUser != null ? t.FromWallet.ApplicationUser.UserName : "System/Stripe",
                    ToUserName = t.ToWallet != null && t.ToWallet.ApplicationUser != null ? t.ToWallet.ApplicationUser.UserName : "System/Stripe",
                    Amount = t.Amount,
                    PlatformFee = t.PlatformFee,
                    CreatedAt = t.CreatedAt,
                    SessionId = t.SessionId ?? 0
                })
                .FirstOrDefaultAsync();
        }

        // ═══════════════════════════════════════════════════════
        // SUBJECTS
        // ═══════════════════════════════════════════════════════
        public async Task<List<AdminSubjectDto>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .Select(s => new AdminSubjectDto
                {
                    SubjectId = s.subjectId,
                    Name = s.name,
                    Description = s.description,
                    TeacherCount = s.Teachers.Count
                })
                .ToListAsync();
        }

        public async Task<Subject> CreateSubjectAsync(CreateSubjectDto dto)
        {
            var subject = new Subject
            {
                name = dto.Name,
                description = dto.Description
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                subject.name = dto.Name;

            if (dto.Description != null)
                subject.description = dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSubjectAsync(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}