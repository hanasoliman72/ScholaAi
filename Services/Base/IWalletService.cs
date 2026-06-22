using ScholaAi.Models;
using ScholaAi.DTOs;
using ScholaAi.DTOs.Payments;

namespace ScholaAi.Services.Base
{
    public interface IWalletService
    {
        Task<Wallet> GetOrCreateWalletAsync(string userId);
        Task CreditWalletAsync(string userId, decimal amount);
        //Task DebitWalletAsync(string userId, decimal amount);
        Task RecordTransactionAsync(string fromUserId, string toUserId, int sessionId, decimal amount, decimal platformFee);
        Task RecordTopupAsync(string userId, decimal amount);
        Task<List<UserTransactionDto>> GetTransactionsByUserIdAsync(string userId);
    }
}
