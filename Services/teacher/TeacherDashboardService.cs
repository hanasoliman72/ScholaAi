using ScholaAi.DTOs.Student;
using ScholaAi.DTOs.Teacher;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;
using ScholaAi.Models;

namespace ScholaAi.Services.teacher
{
    public class TeacherDashboardService : ITeacherDashboardService
    {
        private readonly ITeacherDashboardRepository _repository;
        private readonly IRatingService _ratingService;

        public TeacherDashboardService(
            ITeacherDashboardRepository repository,
            IRatingService ratingService)
        {
            _repository = repository;
            _ratingService = ratingService;
        }

        public async Task<TeacherDashboardDto> GetTeacherDashboardAsync(string teacherId)
        {
            var teacher = await _repository.GetTeacherDashboardAsync(teacherId);
            if (teacher == null)
                throw new Exception("Teacher not found");

            var now = DateTime.UtcNow;

            // ============ EARNINGS ============
            var transactions = teacher.ApplicationUser.Wallet?.TransactionsTo;

            decimal todayEarnings = transactions != null
                ? transactions
                    .Where(t => t.CreatedAt.Date == now.Date)
                    .Sum(t => t.Amount)
                : 0;

            decimal thisMonthEarnings = transactions != null
                ? transactions
                    .Where(t => t.CreatedAt.Month == now.Month && t.CreatedAt.Year == now.Year)
                    .Sum(t => t.Amount)
                : 0;

            decimal weeklyEarnings = transactions != null
                ? transactions
                    .Where(t => t.CreatedAt >= now.AddDays(-7))
                    .Sum(t => t.Amount)
                : 0;

            decimal lastMonthEarnings = transactions != null
                ? transactions
                    .Where(t => t.CreatedAt.Month == now.AddMonths(-1).Month)
                    .Sum(t => t.Amount)
                : 0;

            // ============ UPCOMING SESSIONS ============
            // Exclude requests that already have an active session
            var activeRequestIds = teacher.Sessions != null
                ? new HashSet<int>(teacher.Sessions
                    .Where(s => s.Status == "active")
                    .Select(s => s.RequestId))
                : new HashSet<int>();

            var upcomingSessions = teacher.SessionRequests != null
                ? teacher.SessionRequests
                    .Where(sr =>
                        sr.Status == RequestStatus.Accepted &&
                        sr.PreferredDate > now &&
                        !activeRequestIds.Contains(sr.RequestId)
                    )
                    .OrderBy(sr => sr.PreferredDate)
                    .Select(sr => new TeacherUpcomingSessionDto
                    {
                        RequestId = sr.RequestId,
                        StudentName = sr.Student?.ApplicationUser?.UserName ?? "Unknown Student",
                        SubjectName = sr.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = sr.PreferredDate
                    })
                    .ToList()
                : new List<TeacherUpcomingSessionDto>();

            // ============ ACTIVE SESSIONS ============
            var activeSessions = teacher.Sessions != null
                ? teacher.Sessions
                    .Where(s => s.Status == "active")
                    .Select(s => new TeacherActiveSessionDto
                    {
                        StudentName = s.Student?.ApplicationUser?.UserName ?? "Unknown Student",
                        SubjectName = s.SessionRequest?.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = s.SessionRequest!.FinalScheduledAt ?? s.StartedAt ?? now,
                        sessionId = s.SessionId
                    })
                    .ToList()
                : new List<TeacherActiveSessionDto>();

            // ============ RECENT SESSIONS (last 3 ended) ============
            var recentSessions = teacher.Sessions != null
                ? teacher.Sessions
                    .Where(s => s.Status == "ended" && s.SessionRequest != null)
                    .OrderByDescending(s => s.EndedAt ?? s.SessionRequest.FinalScheduledAt)
                    .Take(3)
                    .Select(s => new TeacherRecentSessionDto
                    {
                        StudentName = s.Student?.ApplicationUser?.UserName ?? "Unknown Student",
                        SubjectName = s.SessionRequest?.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = s.SessionRequest?.FinalScheduledAt ?? s.EndedAt ?? now,
                        StudentFocusScore = s.FocusScore
                    })
                    .ToList()
                : new List<TeacherRecentSessionDto>();

            // ============ AVERAGE RATING ============
            var avgRating = await _ratingService.getTeacherAverageRatingAsync(teacherId);

            // ============ TODAY'S OVERVIEW ============
            var today = now.Date;

            // All sessions (active or ended) that started today
            var todaysSessions = teacher.Sessions != null
                ? teacher.Sessions
                    .Where(s =>
                        (s.Status == "active" || s.Status == "ended") &&
                        s.StartedAt.HasValue &&
                        s.StartedAt.Value.Date == today)
                    .ToList()
                : new List<Session>();

            int sessionsToday = todaysSessions.Count;

            // Hours taught = sum of actual session durations for ended sessions today
            double hoursTaught = todaysSessions
                .Where(s => s.Status == "ended" && s.StartedAt.HasValue && s.EndedAt.HasValue)
                .Sum(s => (s.EndedAt!.Value - s.StartedAt!.Value).TotalHours);

            double avgFocusToday = todaysSessions
                .Where(s => s.FocusScore.HasValue)
                .Select(s => (double)s.FocusScore!.Value)
                .DefaultIfEmpty(0)
                .Average();

            // ============ RETURN DASHBOARD ============
            return new TeacherDashboardDto
            {
                TeacherName = teacher.ApplicationUser?.UserName ?? "Unknown Teacher",
                TodayEarnings = todayEarnings,
                ThisMonthEarnings = thisMonthEarnings,
                AvgRating = avgRating.averageRating,
                UpcomingSessions = upcomingSessions,
                ActiveSessions = activeSessions,
                RecentSessions = recentSessions,
                EarningsSummary = new TeacherEarningsSummaryDto
                {
                    ThisWeek = weeklyEarnings,
                    LastMonth = lastMonthEarnings
                },
                TodayOverview = new TodayOverviewDto
                {
                    SessionsToday = sessionsToday,
                    HoursTaught = Math.Round(hoursTaught, 1),
                    AvgFocusScore = Math.Round(avgFocusToday, 1)
                }
            };
        }
    }
}
