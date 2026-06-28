using System;

namespace ScholaAi.DTOs.Sessions
{
    public class teacherSessionDto
    {
        public int id { get; set; }
        public string subject { get; set; } = string.Empty;
        public string lessonTitle { get; set; } = string.Empty;
        public string student { get; set; } = string.Empty;
        public string date { get; set; } = string.Empty;
        public string duration { get; set; } = string.Empty;
        public int? focusScore { get; set; }
        public string status { get; set; } = string.Empty;
        public string? recordedSession { get; set; }
        public string? summary { get; set; }
    }
}
