using ScholaAi.DTOs.Student;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.Student
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IStudentDashboardRepository _repository;

        public StudentDashboardService(IStudentDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId)
        {
            var student = await _repository.GetStudentDashboardAsync(studentId);
            if (student == null)
                throw new Exception("Student not found");

            var now = DateTime.UtcNow;

            // Avg Focus Score
            var completedSessions = student.Sessions
                .Where(s => s.FocusScore.HasValue)
                .ToList();

            double avgFocus = completedSessions.Any()
                ? completedSessions.Average(s => s.FocusScore!.Value)
                : 0;

            // Sessions this month
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            int sessionsThisMonth = student.Sessions.Count(s =>
                s.SessionRequest.FinalScheduledAt.HasValue &&
                s.SessionRequest.FinalScheduledAt.Value.Month == currentMonth &&
                s.SessionRequest.FinalScheduledAt.Value.Year == currentYear
            );

            // Upcoming Sessions — future accepted requests that do NOT yet have an active session
            var activeRequestIds = student.Sessions != null
                ? new HashSet<int>(student.Sessions
                    .Where(s => s.Status == "active")
                    .Select(s => s.RequestId))
                : new HashSet<int>();

            var upcomingSessions = student.SessionRequests != null
                ? student.SessionRequests
                    .Where(sr =>
                        sr.Status == RequestStatus.Accepted &&
                        sr.PreferredDate > DateTime.UtcNow &&
                        !activeRequestIds.Contains(sr.RequestId)
                    )
                    .OrderBy(sr => sr.PreferredDate)
                    .Select(sr => new UpcomingSessionDto
                    {
                        RequestId = sr.RequestId,
                        TeacherName = sr.Teacher?.ApplicationUser?.UserName ?? "Unknown Teacher",
                        SubjectName = sr.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = sr.PreferredDate
                    })
                    .ToList()
                : new List<UpcomingSessionDto>();

            // ACTIVE SESSIONS
            var activeSessions = student.Sessions != null
                ? student.Sessions
                    .Where(s =>
                        s.Status == "active"
                    )
                    .Select(s => new ActiveSessionDto
                    {
                        TeacherName = s.Teacher?.ApplicationUser?.UserName ?? "Unknown Teacher",
                        SubjectName = s.SessionRequest?.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = s.SessionRequest?.FinalScheduledAt ?? s.StartedAt ?? now,
                        sessionId = s.SessionId
                    })
                    .ToList()
                : new List<ActiveSessionDto>();

            // Recent Sessions (last 3 ended sessions)
            var recentSessions = student.Sessions != null
                ? student.Sessions
                    .Where(s => s.Status == "ended" && s.SessionRequest != null)
                    .OrderByDescending(s => s.EndedAt ?? s.SessionRequest.FinalScheduledAt)
                    .Take(3)
                    .Select(s => new RecentSessionDto
                    {
                        TeacherName = s.Teacher.ApplicationUser.FirstName + " " + s.Teacher.ApplicationUser.LastName,
                        SubjectName = s.SessionRequest.Subject.name,
                        ScheduledAt = s.SessionRequest.FinalScheduledAt ?? s.EndedAt ?? DateTime.UtcNow,
                        FocusScore = s.FocusScore
                    })
                    .ToList()
                : new List<RecentSessionDto>();

            // Weekly Engagement
            var weeklyEngagement = completedSessions
                .Where(s =>
                    s.SessionRequest.FinalScheduledAt.HasValue &&
                    s.SessionRequest.FinalScheduledAt.Value >= DateTime.UtcNow.AddDays(-7)
                )
                .GroupBy(s => s.SessionRequest.FinalScheduledAt!.Value.Date)
                .Select(g => new WeeklyEngagementDto
                {
                    Day = g.Key.ToString("dddd"), // Monday Tuesday
                    AvgFocusScore = g.Average(s => s.FocusScore!.Value)
                })
                .OrderBy(d => d.Day)
                .ToList();

            // Wallet Summary
            var wallet = student.ApplicationUser.Wallet;

            var lastRecharge = wallet?.TransactionsFrom
                .Where(t => t.Amount > 0)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            var lastSessionPayment = wallet?.TransactionsFrom
                .Where(t => t.Amount < 0)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            var walletSummary = new WalletSummaryDto
            {
                LastRechargeAmount = lastRecharge?.Amount ?? 0,
                LastRechargeDate = lastRecharge?.CreatedAt,
                LastSessionAmount = lastSessionPayment?.Amount ?? 0,
                LastSessionDate = lastSessionPayment?.CreatedAt,
            };

            // Final Dto
            return new StudentDashboardDto
            {
                StudentName = student.ApplicationUser.FirstName + " " + student.ApplicationUser.LastName,
                AvgFocusScore = avgFocus,
                SessionsThisMonth = sessionsThisMonth,
                UpcomingSessions = upcomingSessions,
                ActiveSessions = activeSessions,
                RecentSessions = recentSessions,
                WeeklyEngagement = weeklyEngagement,
                WalletBalance = wallet?.Balance ?? 0,
                WalletSummary = walletSummary
            };
        }
    }
}
