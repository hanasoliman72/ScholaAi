using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class session
    {
        [Key]
        public int sessionId { get; set; }
        public int requestId { get; set; }
        public int teacherId { get; set; }
        public int studentId { get; set; }
        public long recordedSession { get; set; }
        public string summary { get; set; }
        [Range(0, 100)]
        public int? focusScore { get; set; }
        public ICollection<notification> notifications { get; set; } = new List<notification>();
        [ForeignKey(nameof(teacherId))]
        public teacher? teacher { get; set; }
        [ForeignKey(nameof(studentId))]
        public student? student { get; set; }
        public rating? rating { get; set; }
        public transaction? transaction { get; set; }
        [ForeignKey(nameof(requestId))]
        public sessionRequest? sessionRequest { get; set; }
    }
}
