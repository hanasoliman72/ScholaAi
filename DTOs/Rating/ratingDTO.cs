using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Rating
{
    public class ratingCreateDto
    {
        [Required]
        public int sessionId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int ratingValue { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string? comment { get; set; }
    }

    public class ratingUpdateDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int ratingValue { get; set; }

        [StringLength(500)]
        public string? comment { get; set; }
    }

    public class ratingDto
    {
        public int ratingId { get; set; }
        public int sessionId { get; set; }
        public int teacherId { get; set; }
        public int? studentId { get; set; }
        public int ratingValue { get; set; }
        public string? comment { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }

    public class teacherAverageRatingDto
    {
        public int teacherId { get; set; }
        public decimal averageRating { get; set; }
        public int totalRatings { get; set; }
    }
}
