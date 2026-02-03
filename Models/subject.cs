using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class subject
    {
        public int subjectId { get; set; }
        public string name { get; set; }
        public string? description { get; set; }

        public ICollection<teacher> teachers { get; set; }
        //public subject? subject { get; set; }
        public ICollection<sessionRequest> sessionRequests { get; set; }

    }

}