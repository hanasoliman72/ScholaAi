namespace ScholaAi.DTOs.Sessions
{
    public class studentRequestDto
    {
        public int sessionId { get; set; }
        public string subject { get; set; }
        public string status { get; set; }
        public string? teacherName { get; set; }
        public DateTime preferredDate { get; set; }
    }
}
