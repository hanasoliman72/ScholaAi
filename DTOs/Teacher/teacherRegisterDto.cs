using ScholaAi.DTOs.Common;
using ScholaAi.Models;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Teacher
{
    public class TeacherRegisterDto
    {
        // ===== Identity =====
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Phone]
        [RegularExpression(@"^\+?[0-9\s\-]{7,20}$", ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string Phone { get; set; }

        // ===== Profile =====
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? Description { get; set; }
        public string? ProfilePhotoURL { get; set; }

        // ===== Teacher =====
        [Required]
        public string College { get; set; }

        [Required]
        public string Certificate { get; set; }

       
        public string TeachingExperience { get; set; }

        public string IdNumber { get; set; }

        public int SubjectId { get; set; }

        // ===== Availability =====
        [MinLength(1, ErrorMessage = "Select at least 1 time slot")]
        public List<availabilityDto> Availability { get; set; } = new List<availabilityDto>();
    }
}
