using ScholaAi.Models;

namespace ScholaAi.DTOs.Sessions
{
    public class createSessionRequestDto
    {
        public int subjectId { get; set; }
        public DateTime preferredDate { get; set; } 
        public string? description { get; set; }
    }
}
