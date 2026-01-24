using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs
{
    public class loginDto
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
