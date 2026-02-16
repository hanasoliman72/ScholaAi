using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public enum UserType
    {
        Student,
        Teacher,
        Admin
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class ApplicationUser : IdentityUser
    {
        // ===== Core Profile =====
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public Gender Gender { get; set; }
        public UserType UserType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ProfilePhotoURL { get; set; }

        // ===== Navigation =====
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        public Wallet? Wallet { get; set; }

        public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
        public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
        public ICollection<AdminLogs> AdminActions { get; set; } = new List<AdminLogs>();
        public ICollection<AdminLogs> AdminTargets { get; set; } = new List<AdminLogs>();

      
        
    }
}