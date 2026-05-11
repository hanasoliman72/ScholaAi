namespace ScholaAi.DTOs.Teacher
{
    public class MyStudentsSummaryDto
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int PreviousStudents { get; set; }
        public int TotalSessions { get; set; }
        public decimal TotalHoursTaught { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class StudentCardDto
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public string SubjectName { get; set; }

        // Stats
        public int TotalSessions { get; set; }
        public decimal TotalHours { get; set; }
        public double? AverageFocusScore { get; set; }

        // Session dates
        public DateTime? LastSessionDate { get; set; }
        public string LastSessionAgo { get; set; }
        public DateTime? NextSessionDate { get; set; }
        public string? NextSessionTime { get; set; }

        public bool IsActive { get; set; }
    }

    public class MyStudentsListResponseDto
    {
        public MyStudentsSummaryDto Summary { get; set; }
        public List<StudentCardDto> ActiveStudents { get; set; } = new();
        public List<StudentCardDto> PreviousStudents { get; set; } = new();
    }

    public class StudentProgressDto
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public string SubjectName { get; set; }

        // Overall stats
        public int TotalSessions { get; set; }
        public decimal TotalHours { get; set; }
        public double? AverageFocusScore { get; set; }
        public DateTime? FirstSessionDate { get; set; }
        public DateTime? LastSessionDate { get; set; }

        // Focus trend (last 5 sessions)
        public List<SessionFocusTrendDto> FocusTrend { get; set; } = new();

        // Session history
        public List<StudentSessionHistoryDto> SessionHistory { get; set; } = new();

        // Upcoming sessions
        public List<StudentUpcomingSessionDto> UpcomingSessions { get; set; } = new();
    }

    public class SessionFocusTrendDto
    {
        public int SessionNumber { get; set; }
        public DateTime Date { get; set; }
        public int FocusScore { get; set; }
    }

    public class StudentSessionHistoryDto
    {
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }
        public int? FocusScore { get; set; }
        public string Status { get; set; }
        public string? Summary { get; set; }
    }

    public class StudentUpcomingSessionDto
    {
        public int SessionId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }
    }
}