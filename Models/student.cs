using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class Student
    {
        [Key]
        public string ApplicationUserId { get; set; }

        public int Grade { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<SessionRequest> SessionRequests { get; set; } = new List<SessionRequest>();
    }
}
