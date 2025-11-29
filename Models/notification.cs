using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public enum notificationType
    {
        Request,
        Session,
        Chat,
        System
    }
    public class notification
    {
        [Key]
        public int notificationId { get; set; }
 
        public int receiverId { get; set; }
        public int senderId { get; set; }
        public int sessionId { get; set; }
        public string message { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public bool isRead { get; set; } = false;
        public notificationType type { get; set; }

        public user? sender { get; set; }
        public user? receiver { get; set; }
        [ForeignKey(nameof(sessionId))]
        public session? sessionNotification { get; set; }

    }
}
