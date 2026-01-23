using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public enum Type
    {
        Student,
        Teacher,
        Admin
    }
    public class user
    {
        public string? applicationUserId { get; set; }

        [Key]
        public int userId { get; set; }
        
        public string userName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }
        [RegularExpression(@"^\+?[0-9\s\-]{7,20}$", ErrorMessage = "Invalid phone number format.")]
        [MaxLength(20)]
        public string phone { get; set; }
        public string? description { get; set; }
        //public string? passwordHash { get; set; } // Not used with Identity - kept for backward compatibility
        public Type userType { get; set; }
        public string? profilePhotoURL { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public ICollection<notification> sentNotifications { get; set; } = new List<notification>();
        public ICollection<notification> receivedNotifications { get; set; } = new List<notification>();

        public ICollection<chatMessage> sentMessages { get; set; } = new List<chatMessage>();
        public ICollection<chatMessage> receivedMessages { get; set; } = new List<chatMessage>();
        public ICollection<adminLogs> admins { get; set; } = new List<adminLogs>();

        public adminLogs? adminLogs { get; set; }
        // Navigation
        public applicationUser applicationUser { get; set; }


        public student? student { get; set; }
        public teacher? teacher { get; set; }
        public wallet? wallet { get; set; }
    }
}
