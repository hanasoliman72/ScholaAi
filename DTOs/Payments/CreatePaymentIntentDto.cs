using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Payments
{
    public class CreatePaymentIntentDto
    {
        [Required]
        [Range(1, 100000)]
        public decimal Amount { get; set; }
    }
}
