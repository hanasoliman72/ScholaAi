using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class subject
    {
        [Key]
        public int subjectId { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public ICollection<teacherSubject> teacherSubjects { get; set; } = new List<teacherSubject>();
        public sessionRequest? sessionRequest { get; set; }
    }

}
