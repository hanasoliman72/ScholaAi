namespace ScholaAi.Models
{
    public enum TimeSlot
    {
        Morning,    // 6–12
        Afternoon,  // 12–17
        Evening     // 17–22
    }
    public class availability
    {
        public int id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int userId { get; set; }

        public user user { get; set; }
    }
}
