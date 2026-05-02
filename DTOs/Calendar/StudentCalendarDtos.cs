namespace ScholaAi.DTOs.Calendar
{
    public class StudentCalendarSessionDto
    {
        public int SessionId { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Time { get; set; }        // "3:00 PM"
        public string Duration { get; set; }    // "1 hour"
        public string Status { get; set; }      // "Upcoming", "Completed", "Pending"
        public int? FocusScore { get; set; }
        public bool HasNotes { get; set; }
    }

    public class StudentCalendarDayDto
    {
        public DateTime Date { get; set; }
        public List<StudentCalendarSessionDto> Sessions { get; set; } = new();
    }

    public class StudentCalendarMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        // Summary
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int UpcomingSessions { get; set; }

        // Which dates have sessions and what color dot
        // Key = day number (1-31), Value = status color
        public Dictionary<int, string> DatesWithSessions { get; set; } = new();
    }

    public class StudentCalendarDayDetailDto
    {
        public DateTime Date { get; set; }
        public List<StudentCalendarSessionDto> Sessions { get; set; } = new();

        // Summary for the month
        public int TotalSessionsThisMonth { get; set; }
        public int CompletedThisMonth { get; set; }
        public int UpcomingThisMonth { get; set; }
    }
    public class StudentSessionNotesDto
    {
        public int SessionId { get; set; }
        public string Summary { get; set; }
        public int? FocusScore { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public DateTime ScheduledAt { get; set; }
    }
}
