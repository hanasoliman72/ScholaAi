namespace ScholaAi.DTOs.Teatcher
{
    public class teacherProfileDto
    {
        // Basic ApplicationUser info
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? description { get; set; }
        public string? profilePhotoURL { get; set; }

        // Teacher specific info
        public string college { get; set; }
        public string certificate { get; set; }
        public string teachingExperience { get; set; }

        // Statistics
        public decimal totalHoursTaught { get; set; }
        public decimal averageRate { get; set; }
        public int totalRatings { get; set; }
        public int totalSessions { get; set; }
    }
}
