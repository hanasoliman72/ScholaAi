using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Common
{
    public class changePasswordDto
    {
        [Required]
        public string currentPassword {  get; set; }
        [Required]
        [MinLength(6)]
        public string newPassword { get; set; }
        [Required]
        [Compare("newPassword")]
        public string confirmPassword { get; set; }
    }
}
