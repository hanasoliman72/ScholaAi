namespace ScholaAi.DTOs.Sessions
{
    public class teacherRequestDto
    {
        public int sessionId { get; set; }
        public string studentName { get; set; }
        public string subject { get; set; }
        public DateTime preferredDate { get; set; }
        public string? description { get; set; }
    }
}
