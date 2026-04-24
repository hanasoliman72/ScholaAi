namespace ScholaAi.DTOs.Teacher
{
    public class TeacherDashboardDto
    {
        public string TeacherName { get; set; }

        public decimal TodayEarnings { get; set; }
        public decimal ThisMonthEarnings { get; set; }

        public decimal AvgRating { get; set; }


        public List<TeacherUpcomingSessionDto> UpcomingSessions { get; set; } = new();
        public List<TeacherRecentSessionDto> RecentSessions { get; set; } = new();

        public TeacherEarningsSummaryDto EarningsSummary { get; set; }
    }

    public class TeacherUpcomingSessionDto
    {
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
