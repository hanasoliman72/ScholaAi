
namespace ScholaAi.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int ActiveSessions { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalSessionsThisMonth { get; set; }
    }
}