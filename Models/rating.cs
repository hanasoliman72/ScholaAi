using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class rating
    {
        [Key]
        public int ratingId { get; set; }
        public int sessionId { get; set; }
        public int teacherId { get; set; }
        public int ratingValue { get; set; }

        [ForeignKey(nameof(teacherId))]
        public teacher? teacher { get; set; }
        [ForeignKey(nameof(sessionId))]
        public session? session { get; set; }
    }
}
