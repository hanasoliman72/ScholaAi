using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public enum requestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }
    public class sessionRequest
    {
        [Key]
        public int sessionId { get; set; }
        public int? teacherId { get; set; }
        public int studentId { get; set; }
        public int subjectId { get; set; }
        public DateTime preferredDate { get; set; } = DateTime.Now;
        public requestStatus status { get; set; }
        public string? description { get; set; }
        public DateTime? finalScheduledAt { get; set; }
        public DateTime createdAt { get; set; }=DateTime.Now;
        public adminLogs? adminLogs { get; set; }
        [ForeignKey(nameof(teacherId))]
        public teacher? teacher { get; set; }
        [ForeignKey(nameof(studentId))]
        public student? student { get; set; }
        [ForeignKey(nameof(subjectId))]
        public subject? subject { get; set; }
        public session? session { get; set; }
        public ICollection<requestBroadcast> requestBroadcasts { get; set; } = new List<requestBroadcast>();
        
    }
}
