using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public enum RequestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }

    public class SessionRequest
    {
        [Key]
        public int RequestId { get; set; }

        // FK → Teacher (nullable until accepted)
        public string? TeacherId { get; set; }

        // FK → Student
        [Required]
        public string StudentId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public DateTime PreferredDate { get; set; } = DateTime.UtcNow;
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string? Description { get; set; }
        public DateTime? FinalScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AdminLogs? AdminLogs { get; set; }

        // Navigation properties
        public Teacher? Teacher { get; set; }
        public Student Student { get; set; }
        public Subject Subject { get; set; }
        public Session? Session { get; set; }
        public ICollection<RequestBroadcast> RequestBroadcasts { get; set; } = new List<RequestBroadcast>();
    }
}