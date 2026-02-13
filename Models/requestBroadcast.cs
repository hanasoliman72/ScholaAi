using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class RequestBroadcast
    {
        [Key]
        public int BroadcastId { get; set; }

        // FK → SessionRequest
        [Required]
        public int RequestId { get; set; }

        // FK → Teacher (App)
        [Required]
        public string TeacherId { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsDelivered { get; set; } = false;

        public bool IsAccepted { get; set; } = false;

        // Navigation properties
        [ForeignKey(nameof(TeacherId))]
        public ApplicationUser Teacher { get; set; }

        [ForeignKey(nameof(RequestId))]
        public SessionRequest TeacherSession { get; set; }
    }
}
