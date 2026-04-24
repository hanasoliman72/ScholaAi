
namespace ScholaAi.DTOs.Admin
{
    public class AdminSessionListDto
    {
        public int SessionId { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? FocusScore { get; set; }
        public bool IsLive { get; set; }
    }

    public class AdminSessionDetailDto : AdminSessionListDto
    {
        public string TeacherId { get; set; }
        public string StudentId { get; set; }
        public string Summary { get; set; }
        public long RecordedSessionSeconds { get; set; }
        public decimal? TransactionAmount { get; set; }
    }
}