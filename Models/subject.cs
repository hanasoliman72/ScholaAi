using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class Subject
    {
        public int subjectId { get; set; }
        public string name { get; set; }
        public string? description { get; set; }

        public ICollection<Teacher> Teachers { get; set; }
        //public subject? subject { get; set; }
        public ICollection<SessionRequest> sessionRequests { get; set; }

    }

}