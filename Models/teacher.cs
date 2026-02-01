using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class teacher
    {
        [Key]
        public int userId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string college { get; set; }
        [MaxLength(500)]
        public string? description { get; set; }
        [Required]
        public string certificate { get; set; }
        [Required]
        public string teachingExperience { get; set; }
        [Required]
        public string subjectName { get; set; }
        [Precision(18, 4)]
        public decimal totalHoursTaught { get; set; } = 0;
        [Precision(18, 4)]
        public decimal totalRates { get; set; } = 0;

        public user? user { get; set; }

        public ICollection<teacherSubject> teacherSubjects { get; set; } = new List<teacherSubject>();
        public ICollection<rating> ratings { get; set; } = new List<rating>();
        public ICollection<sessionRequest> sessionRequests { get; set; } = new List<sessionRequest>();
        public ICollection<requestBroadcast> requestBroadcasts { get; set; } = new List<requestBroadcast>();
        public ICollection<session> sessions { get; set; } = new List<session>();
        
    }
}
