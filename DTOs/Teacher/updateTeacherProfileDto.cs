using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Teacher
{
    public class updateTeacherProfileDto
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
        [RegularExpression(@"^\+?[0-9\s\-]{7,20}$", ErrorMessage = "Invalid phone number format.")]
        public string? phone { get; set; }

        [StringLength(500)]
        public string? description { get; set; }

        // Teacher specific fields
        [StringLength(200)]
        public string? college { get; set; }

        [StringLength(200)]
        public string? certificate { get; set; }

        [StringLength(500)]
        public string? teachingExperience { get; set; }
    }
}