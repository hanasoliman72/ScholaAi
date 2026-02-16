using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }

        public int RequestId { get; set; }

        // FK → Identity Users
        [Required]
        public string TeacherId { get; set; }

        [Required]
        public string StudentId { get; set; }

        public long RecordedSession { get; set; }

        public string Summary { get; set; }

        [Range(0, 100)]
        public int? FocusScore { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        // Navigation
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        public Rating Rating { get; set; }

        public Transaction Transaction { get; set; }

        [ForeignKey(nameof(RequestId))]
        public SessionRequest SessionRequest { get; set; }
    }
}
