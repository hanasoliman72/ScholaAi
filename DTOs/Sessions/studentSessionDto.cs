using System;

namespace ScholaAi.DTOs.Sessions
{
    public class studentSessionDto
    {
        public int id { get; set; }
        public string subject { get; set; } = string.Empty;
        public string lessonTitle { get; set; } = string.Empty;
        public string teacher { get; set; } = string.Empty;
        public string teacherInitials { get; set; } = string.Empty;
        public string date { get; set; } = string.Empty;
        public string duration { get; set; } = string.Empty;
        public int? focusScore { get; set; }
        public string status { get; set; } = string.Empty;
        public string? recordedSession { get; set; }
        public string? summary { get; set; }
        public int? ratingId { get; set; }
        public int? ratingValue { get; set; }
    }
}
