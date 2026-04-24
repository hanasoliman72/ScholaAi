using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public enum NotificationType
    {
        Request,
        Session,
        Chat,
        System
    }

    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public int? SessionId { get; set; }

        public int? RequestId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public NotificationType Type { get; set; }

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser Receiver { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session? SessionNotification { get; set; }

        [ForeignKey(nameof(RequestId))]
        public SessionRequest? SessionRequestReference { get; set; }
    }
}
