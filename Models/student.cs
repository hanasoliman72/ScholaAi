using System.ComponentModel.DataAnnotations;

namespace ScholaAi.Models
{
    public class student
    {
        [Key]
        public int userId { get; set; }
        public int grade { get; set; }
        public user? user { get; set; }
        public ICollection<session> sessions { get; set; } = new List<session>();
        public ICollection<sessionRequest> requests { get; set; }= new List<sessionRequest>();

    }
}
