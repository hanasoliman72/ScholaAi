namespace ScholaAi.DTOs.Payments
{
    public class UserTransactionDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "credit"; // "credit" or "debit"
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
