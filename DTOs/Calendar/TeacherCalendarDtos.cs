namespace ScholaAi.DTOs.Calendar
{
    public class TeacherCalendarSessionDto
    {
        public int SessionId { get; set; }
        public string SubjectName { get; set; }
        public string StudentName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Time { get; set; }        // "3:00 PM"
        public string Duration { get; set; }    // "1 hour"
        public string Status { get; set; }      // "Upcoming", "Completed", "Pending"
        public int? FocusScore { get; set; }
    }

    public class TeacherCalendarMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        // Summary
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int UpcomingSessions { get; set; }

        // Key = day number (1-31), Value = status color
        // blue = upcoming, green = completed, yellow = pending
        public Dictionary<int, string> DatesWithSessions { get; set; } = new();
    }

    public class TeacherCalendarDayDetailDto
    {
        public DateTime Date { get; set; }
        public List<TeacherCalendarSessionDto> Sessions { get; set; } = new();

        // Summary for the month
        public int TotalSessionsThisMonth { get; set; }
        public int CompletedThisMonth { get; set; }
        public int UpcomingThisMonth { get; set; }
    }

    public class TeacherSessionAnalysisDto
    {
        public int SessionId { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int? FocusScore { get; set; }
        public string Summary { get; set; }
        public long RecordedSessionSeconds { get; set; }
        public string Duration { get; set; }
    }
}