using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class Teacher
    {
        [Key]
        public string ApplicationUserId { get; set; }

        [Required]
        public string College { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Certificate { get; set; }

        [Required]
        public string TeachingExperience { get; set; }

        [Precision(18, 4)]
        public decimal TotalHoursTaught { get; set; } = 0;

        [Precision(18, 4)]
        public decimal TotalRates { get; set; } = 0;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<SessionRequest> SessionRequests { get; set; } = new List<SessionRequest>();
        public ICollection<RequestBroadcast> RequestBroadcasts { get; set; } = new List<RequestBroadcast>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
