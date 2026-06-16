namespace ScholaAi.DTOs.Sessions
{
    public class teacherRequestDto
    {
        public int sessionId { get; set; }
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string subject { get; set; }
        public DateTime preferredDate { get; set; }
        public string? description { get; set; }
        public bool isAccepted { get; set; }
    }
}
