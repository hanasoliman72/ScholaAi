using Microsoft.Extensions.Options;
using ScholaAi.DTOs.Payments;
using ScholaAi.Models;
using ScholaAi.Services.Base;
using ScholaAi.Models;
using Stripe;



namespace ScholaAi.Services.payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IWalletService _walletService;
        private readonly IOptions<StripeSettings> _stripeSettings;

        public PaymentService(IWalletService walletService, IOptions<StripeSettings> stripeSettings)
        {
            _walletService = walletService;
            _stripeSettings = stripeSettings;
        }

        IWalletService WalletService { get; }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(string userId, decimal amount)
        {
            //var options = new PaymentIntentCreateOptions
            //{
            //    Amount = (long)(amount * 100),
            //    Currency = "usd",
            //    Metadata = new Dictionary<string, string>
            //{
            //    { "userId", userId },
            //    { "type", "wallet_topup" }
            //}
            //};
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",

                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                },

                Metadata = new Dictionary<string, string>
    {
        { "userId", userId },
        { "type", "wallet_topup" }
    }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new PaymentIntentResponseDto
            {
                ClientSecret = intent.ClientSecret,
                PublishableKey = _stripeSettings.Value.PublishableKey
            };
        }

        public async Task<bool> HandleWebhookAsync(string json, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json, signature, _stripeSettings.Value.WebhookSecret);

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;

                    if (intent == null)
                        return false;

                    if (intent.Metadata == null || !intent.Metadata.ContainsKey("userId"))
                        return false;

                    var userId = intent.Metadata["userId"];

                    var amount = intent.Amount / 100m;

                    await _walletService.CreditWalletAsync(userId, amount);
                    await _walletService.RecordTopupAsync(userId, amount);


                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return false;
            }
        }
    }
}
