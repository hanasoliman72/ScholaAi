
namespace ScholaAi.DTOs.Admin
{
    public class AdminLogDto
    {
        public int LogId { get; set; }
        public string AdminName { get; set; }
        public string? TargetUserName { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}