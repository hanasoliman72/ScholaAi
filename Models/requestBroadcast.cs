using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class requestBroadcast
    {
        [Key]
        public int broadcastId { get; set; }
        public int requestId { get; set; }
        public int teacherId { get; set; }
        public DateTime sentAt { get; set; }= DateTime.UtcNow;
        public bool isDelivered { get; set; }= false;
        public bool isAccepted { get; set; } = false;
        [ForeignKey(nameof(teacherId))]
        public teacher teacher { get; set; }
        [ForeignKey(nameof(requestId))]
        public sessionRequest? teacherSession { get; set; }
    }
}
