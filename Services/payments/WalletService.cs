using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;

namespace ScholaAi.Services.payments
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task CreditWalletAsync(string userId, decimal amount)
        {
            var wallet = await GetOrCreateWalletAsync(userId);
            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.updateAsync(wallet); 
            await _walletRepository.SaveChangesAsync();
        
        }

        //public async Task DebitWalletAsync(string userId, decimal amount)
        //{
        //    var wallet = await GetOrCreateWalletAsync(userId);
        //    if (wallet.Balance < amount)
        //        throw new InvalidOperationException("Insufficient wallet balance.");

        //    wallet.Balance -= amount;
        //    wallet.UpdatedAt = DateTime.UtcNow;
        //    await _walletRepository.updateAsync(wallet);              
        //    await _walletRepository.SaveChangesAsync();
        //}

        public async Task<Wallet> GetOrCreateWalletAsync(string userId)
        {
            var wallet = await _walletRepository.getByIdAsync(userId); 
            if (wallet == null)
            {
                wallet = new Wallet { ApplicationUserId = userId, Balance = 0 };
                await _walletRepository.AddAsync(wallet);             
                await _walletRepository.SaveChangesAsync();
            }
            return wallet;
        }

        public async Task RecordTransactionAsync(string fromUserId, string toUserId, int sessionId, decimal amount, decimal platformFee)
        {
            var transaction = new Transaction
            {
                FromWalletId = fromUserId,
                ToWalletId = toUserId,
                SessionId = sessionId,
                Amount = amount,
                PlatformFee = platformFee,
                CreatedAt = DateTime.UtcNow
            };
            await _walletRepository.AddTransactionAsync(transaction); 
            await _walletRepository.SaveChangesAsync();
        }
    }
}
