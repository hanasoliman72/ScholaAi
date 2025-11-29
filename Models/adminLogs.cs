using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class adminLogs
    {
        [Key]
        public int logId { get; set; }
        public int adminId { get; set; }
        public int? targetType { get; set; }
        public int? targetUserId { get; set; }
        public int? targetRequestId { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public string? details { get; set; }

        
        [ForeignKey(nameof(logId))]
        public user? admin { get; set; }
        [ForeignKey(nameof(targetUserId))]
        public user? target { get; set; }
        [ForeignKey(nameof(targetRequestId))]
        public sessionRequest? targetReruest { get; set; }
    }
}
