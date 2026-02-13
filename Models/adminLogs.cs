using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class AdminLogs
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string AdminId { get; set; }

        public string? TargetUserId { get; set; }

        public int? TargetType { get; set; }
        public int? TargetRequestId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; }

        [ForeignKey(nameof(AdminId))]
        public ApplicationUser Admin { get; set; }

        [ForeignKey(nameof(TargetUserId))]
        public ApplicationUser? TargetUser { get; set; }

        [ForeignKey(nameof(TargetRequestId))]
        public SessionRequest? TargetRequest { get; set; }
    }
}
