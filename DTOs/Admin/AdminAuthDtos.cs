
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Admin
{
    public class AdminLoginDto
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }

    public class AdminProfileDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePhotoURL { get; set; }
    }
}