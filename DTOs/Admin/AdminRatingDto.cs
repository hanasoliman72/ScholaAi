namespace ScholaAi.DTOs.Admin
{
    public class AdminRatingDto
    {
        public int RatingId { get; set; }
        public string TeacherName { get; set; }
        public string? StudentName { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}