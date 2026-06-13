namespace ScholaAi.DTOs.Student
{
    public class StudentDashboardDto
    {
        public string StudentName { get; set; }

        public double AvgFocusScore { get; set; }

        public int SessionsThisMonth { get; set; }

        public decimal WalletBalance { get; set; }

        public List<UpcomingSessionDto> UpcomingSessions { get; set; } = new();

        public List<ActiveSessionDto> ActiveSessions { get; set; } = new();

        public List<RecentSessionDto> RecentSessions { get; set; } = new();

        public List<WeeklyEngagementDto> WeeklyEngagement { get; set; } = new();

        public WalletSummaryDto WalletSummary { get; set; }
    }

    public class ActiveSessionDto
    {
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int sessionId { get; set; }
    }

    public class UpcomingSessionDto
    {
        public int RequestId { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public DateTime ScheduledAt { get; set; }
    }

    public class RecentSessionDto
    {
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int? FocusScore { get; set; }
    }

    public class WeeklyEngagementDto
    {
        public string Day { get; set; } // Mon, Tue, etc.
        public double AvgFocusScore { get; set; }
    }

    public class WalletSummaryDto
    {
        public decimal LastRechargeAmount { get; set; }
        public DateTime? LastRechargeDate { get; set; }

        public decimal LastSessionAmount { get; set; }
        public DateTime? LastSessionDate { get; set; }
    }
}
