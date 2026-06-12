using ScholaAi.DTOs.Payments;

namespace ScholaAi.Services.Base
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string userId, decimal amount);
        Task<bool> HandleWebhookAsync(string json, string signature);
    }
}
