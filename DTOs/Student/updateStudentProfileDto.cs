using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Student
{
    public class updateStudentProfileDto
    {
        [StringLength(50)]
        [RegularExpression("^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username can contain only letters, numbers, and underscores.")]
        public string? userName { get; set; }
        [StringLength(50)]
        public string? firstName { get; set; }
        [StringLength(50)]
        public string? lastName { get; set; }
        [Phone]
        [RegularExpression(@"^\+[1-9]\d{1,14}$",
        ErrorMessage = "Phone number must be in valid international format (E.164). Example: +14155552671")]
        public string? phone { get; set; }
        [Range(1, 12, ErrorMessage = "Grade must be between 1 and 12.")]
        public int? grade { get; set; }
        [StringLength(500)]
        public string? description { get; set; }
    }
}
