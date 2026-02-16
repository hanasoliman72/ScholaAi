using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class RequestBroadcast
    {
        [Key]
        public int BroadcastId { get; set; }

        [Required]
        public int RequestId { get; set; }

    
        [Required]
        public string TeacherId { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsDelivered { get; set; } = false;
        public bool IsAccepted { get; set; } = false;

        public Teacher Teacher { get; set; }
        public SessionRequest? SessionRequest { get; set; }
    }
}