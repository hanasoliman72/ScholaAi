namespace ScholaAi.DTOs.Teacher
{
    public class TeacherDashboardDto
    {
        public string TeacherName { get; set; }

        public decimal TodayEarnings { get; set; }
        public decimal ThisMonthEarnings { get; set; }

        public decimal AvgRating { get; set; }

        public List<TeacherActiveSessionDto> ActiveSessions { get; set; } = new();
        public List<TeacherUpcomingSessionDto> UpcomingSessions { get; set; } = new();
        public List<TeacherRecentSessionDto> RecentSessions { get; set; } = new();

        public TeacherEarningsSummaryDto EarningsSummary { get; set; }

        public TodayOverviewDto TodayOverview { get; set; }
    }

    public class TodayOverviewDto
    {
        public int SessionsToday { get; set; }
        public double HoursTaught { get; set; }
        public double AvgFocusScore { get; set; }
    }

    public class TeacherActiveSessionDto
    {
        public string StudentName { get; set; }

        public DateTime ScheduledAt { get; set; }

        public string SubjectName { get; set; }

        public int sessionId { get; set; }
    }

    public class TeacherUpcomingSessionDto
    {
        public int RequestId { get; set; }
        public string StudentName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string SubjectName { get; set; }
    }

    public class TeacherRecentSessionDto
    {
        public string StudentName { get; set; }

        public DateTime ScheduledAt { get; set; }

        public string SubjectName { get; set; }

        public int? StudentFocusScore { get; set; }

    }

    public class TeacherEarningsSummaryDto
    {
        public decimal ThisWeek { get; set; }
        public decimal LastMonth { get; set; }
    }
}
