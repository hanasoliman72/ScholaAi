using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.User
{
    public class userDto
    {
        public string userName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [RegularExpression(@"^\+?[0-9\s\-]{7,20}$", ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string phone { get; set; }
        public string? description { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? profilePhotoURL { get; set; }
       
    }
}
