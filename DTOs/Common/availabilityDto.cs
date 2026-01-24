using ScholaAi.Models;

namespace ScholaAi.DTOs.Common
{
    public class availabilityDto
    {
        public DayOfWeek Day { get; set; }
        public TimeSlot TimeSlot { get; set; }
    }
}
