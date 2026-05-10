using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScholaAi.DTOs.Payments;
using ScholaAi.Models;
using ScholaAi.Services.Base;
using ScholaAi.Services.payments;
using System;
using System.Security.Claims;

namespace ScholaAi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _paymentService;
        private readonly IWalletService _walletService;

        public PaymentController(IPaymentService paymentService, IWalletService walletService)
        {
            _paymentService = paymentService;
            _walletService = walletService;
        }

        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wallet = await _walletService.GetOrCreateWalletAsync(userId);
            return Ok(new { wallet.Balance, wallet.UpdatedAt });
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _paymentService.CreatePaymentIntentAsync(userId, dto.Amount);
            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            Request.Body.Position = 0;
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            var success = await _paymentService.HandleWebhookAsync(json, signature);
            if (!success)
                return BadRequest("Invalid webhook.");

            return Ok();
        }

    }

}
