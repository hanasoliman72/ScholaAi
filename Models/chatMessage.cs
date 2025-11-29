using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class chatMessage
    {
        [Key]
        public int messageId { get; set; }
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public string messageText { get; set; }
        public string? attachmentURL { get; set; }
        public bool isRead { get; set; } = false;
        public DateTime sentAt { get; set; } = DateTime.MinValue;

        [ForeignKey(nameof(senderId))]
        public user? sender { get; set; }
        [ForeignKey(nameof(receiverId))]
        public user? receiver { get; set; }
    }
}
