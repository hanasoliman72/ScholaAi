namespace ScholaAi.DTOs.Student
{
    public class studentProfileDto
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? description { get; set; }
        public string? profilePhotoURL { get; set; }
        public int grade { get; set; }

        // Session statistics
        public int totalSessions { get; set; }
        public decimal totalHours { get; set; }
        public double? averageFocusScore { get; set; }
        public int sessionsThisMonth { get; set; }

        // Subscription info
        public decimal? walletBalance { get; set; }

        public List<PaymentHistoryItemDto> paymentHistory { get; set; } = new();
    }

    public class PaymentHistoryItemDto
    {
        public int transactionId { get; set; }
        // public string description { get; set; }  // EX: "Premium - 20 Hours"
        public decimal amount { get; set; }
        public DateTime date { get; set; }
    }
}
