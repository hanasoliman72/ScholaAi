using ScholaAi.DTOs.Common;
using ScholaAi.Models;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Teatcher
{
    public class teacherRegisterDto
    {
        public int userId { get; set; }
        public string id { get; set; } // Identity user ID (applicationUserId)
        public string userName { get; set; }

        public string? profilePhotoURL { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }
        [RegularExpression(@"^\+?[0-9\s\-]{7,20}$", ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string phone { get; set; } 
        public string college { get; set; }
        public string certificate { get; set; }
        public string? description { get; set; }
        public string subjectName { get; set; }
        [Required]
        public Gender gender { get; set; }
        [Required]
        public string teachingExperience { get; set; }
        [MinLength(1, ErrorMessage = "Select at least 1 time slot")]
        public List<availabilityDto> availability { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
