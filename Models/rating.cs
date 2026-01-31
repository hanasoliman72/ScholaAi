using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class rating
    {
        [Key]
        public int ratingId { get; set; }
        [Required]
        public int sessionId { get; set; }
        [Required]
        public int teacherId { get; set; }
        public int? studentId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int ratingValue { get; set; }
        [StringLength(500)]
        public string? comment { get; set; }
        [Required]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(teacherId))]
        public teacher? teacher { get; set; }
        [ForeignKey(nameof(sessionId))]
        public session? session { get; set; }
        [ForeignKey(nameof(studentId))]
        public user? student { get; set; }
    }
}
