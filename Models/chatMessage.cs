using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        // FIX: Made MessageText nullable. When uploading images, we pass null for text, and EF Core would crash if this was required.
        public string? MessageText { get; set; }
        public string? AttachmentURL { get; set; }

        // FIX: Added MessageType column so the frontend knows whether this message is "text" or "image".
        public string MessageType { get; set; } = "text";

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public ApplicationUser Receiver { get; set; }
    }
}
