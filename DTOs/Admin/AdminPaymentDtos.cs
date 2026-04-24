
namespace ScholaAi.DTOs.Admin
{
    public class AdminPaymentListDto
    {
        public int TransactionId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SessionId { get; set; }
    }
}