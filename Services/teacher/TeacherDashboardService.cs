//using ScholaAi.DTOs.Student;
//using ScholaAi.DTOs.Teacher;
//using ScholaAi.Repositories.Base;
//using ScholaAi.Services.Base;
//using System.Collections.Generic;
//using System.Transactions;

//namespace ScholaAi.Services.teacher
//{
//    public class TeacherDashboardService : ITeacherDashboardService
//    {
//        private readonly ITeacherDashboardRepository _repository;
//        private readonly IRatingService _ratingService;

//        public TeacherDashboardService(ITeacherDashboardRepository repository, IRatingService ratingService)
//        {
//            _repository = repository;
//            _ratingService = ratingService;
//        }

//        public async Task<TeacherDashboardDto> GetTeacherDashboardAsync(string teacherId)
//        {
//            var teacher = await _repository.GetTeacherDashboardAsync(teacherId);

//            if (teacher == null)
//                throw new Exception("Teacher not found");

//            var now = DateTime.UtcNow;

//            // Earnings
//            var transactions = teacher.ApplicationUser.Wallet?.TransactionsTo;

//            var todayEarnings = transactions
//                .Where(t => t.CreatedAt.Date == now.Date)
//                .Sum(t => t.Amount);

//            var thisMonthEarnings = transactions
//                .Where(t => t.CreatedAt.Month == now.Month && t.CreatedAt.Year == now.Year)
//                .Sum(t => t.Amount);

//            var weeklyEarnings = transactions
//                .Where(t => t.CreatedAt >= now.AddDays(-7))
//                .Sum(t => t.Amount);

//            var lastMonthEarnings = transactions
//                .Where(t => t.CreatedAt.Month == now.AddMonths(-1).Month)
//                .Sum(t => t.Amount);

//            // Sessions
//            var upcomingSessions = teacher.Sessions
//                .Where(s => s.SessionRequest.FinalScheduledAt > now)
//                .OrderBy(s => s.SessionRequest.FinalScheduledAt)
//                .Select(s => new TeacherUpcomingSessionDto
//                {
//                    StudentName = s.Student.ApplicationUser.UserName,
//                    SubjectName = s.SessionRequest.Subject.name,
//                    ScheduledAt = s.SessionRequest.FinalScheduledAt!.Value
//                })
//                .ToList();

//            var recentSessions = teacher.Sessions
//                .Where(s => s.FocusScore != null)
//                .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
//                .Take(5)
//                .Select(s => new TeacherRecentSessionDto
//                {
//                    StudentName = s.Student.ApplicationUser.UserName,
//                    SubjectName = s.SessionRequest.Subject.name,
//                    ScheduledAt = s.SessionRequest.FinalScheduledAt!.Value,
//                    StudentFocusScore = s.FocusScore!.Value
//                })
//                .ToList();

//            var avgRating = await _ratingService.getTeacherAverageRatingAsync(teacherId);

//            return new TeacherDashboardDto
//            {
//                TeacherName = teacher.ApplicationUser.UserName,

//                TodayEarnings = todayEarnings,
//                ThisMonthEarnings = thisMonthEarnings,

//                AvgRating = avgRating.averageRating,

//                UpcomingSessions = upcomingSessions,
//                RecentSessions = recentSessions,

//                EarningsSummary = new TeacherEarningsSummaryDto
//                {
//                    ThisWeek = weeklyEarnings,
//                    LastMonth = lastMonthEarnings
//                }
//            };
//        }
//    }
//}
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

            //Console.WriteLine($"Sessions count: {teacher?.Sessions?.Count}");
            //foreach (var s in teacher?.Sessions ?? new List<Session>())
            //{
            //    Console.WriteLine($"  Session {s.SessionId} | Status={s.Status} | SR null={s.SessionRequest == null} | PreferredDate={s.SessionRequest?.PreferredDate} | now={DateTime.UtcNow}");
            //}

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
            var upcomingSessions = teacher.SessionRequests != null
                ? teacher.SessionRequests
                    .Where(sr =>
                        sr.PreferredDate > now &&
                        sr.Status == RequestStatus.Accepted   
                    )
                    .OrderBy(sr => sr.PreferredDate)
                    .Select(sr => new TeacherUpcomingSessionDto
                    {
                        StudentName = sr.Student?.ApplicationUser?.UserName ?? "Unknown Student",
                        SubjectName = sr.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = sr.PreferredDate
                    })
                    .ToList()
                : new List<TeacherUpcomingSessionDto>();

            // ============ COMPLETED SESSIONS (with focus score) ============
            var completedSessions = teacher.Sessions != null
                ? teacher.Sessions
                    .Where(s => s.FocusScore.HasValue && s.SessionRequest != null)
                    .ToList()
                : new List<Models.Session>();

            // ============ RECENT SESSIONS ============
            var recentSessions = completedSessions.Any()
                ? completedSessions
                    .OrderByDescending(s => s.SessionRequest.FinalScheduledAt)
                    .Take(5)
                    .Select(s => new TeacherRecentSessionDto
                    {
                        StudentName = s.Student?.ApplicationUser?.UserName ?? "Unknown Student",
                        SubjectName = s.SessionRequest?.Subject?.name ?? "Unknown Subject",
                        ScheduledAt = s.SessionRequest?.FinalScheduledAt ?? DateTime.UtcNow,
                        StudentFocusScore = s.FocusScore!.Value
                    })
                    .ToList()
                : new List<TeacherRecentSessionDto>();

            // ============ AVERAGE RATING ============
            var avgRating = await _ratingService.getTeacherAverageRatingAsync(teacherId);

            // ============ RETURN DASHBOARD ============
            return new TeacherDashboardDto
            {
                TeacherName = teacher.ApplicationUser?.UserName ?? "Unknown Teacher",
                TodayEarnings = todayEarnings,
                ThisMonthEarnings = thisMonthEarnings,
                AvgRating = avgRating.averageRating,
                UpcomingSessions = upcomingSessions,
                RecentSessions = recentSessions,
                EarningsSummary = new TeacherEarningsSummaryDto
                {
                    ThisWeek = weeklyEarnings,
                    LastMonth = lastMonthEarnings
                }
            };
        }
    }
}
