using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        // FK → Session
        [Required]
        public int SessionId { get; set; }

        // FK → Teacher (App)
        [Required]
        public string TeacherId { get; set; }

        // FK → Student (App) يمكن يكون null لو تقييم عام
        public string? StudentId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int RatingValue { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(TeacherId))]
        public ApplicationUser Teacher { get; set; }

        [ForeignKey(nameof(StudentId))]
        public ApplicationUser? Student { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
