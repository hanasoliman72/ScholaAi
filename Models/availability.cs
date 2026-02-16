using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public enum TimeSlot
    {
        Morning,    // 6–12
        Afternoon,  // 12–17
        Evening     // 17–22
    }

    public class Availability
    {
        [Key]
        public int id { get; set; }

        public DayOfWeek day { get; set; }

        public TimeSlot timeSlot { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
