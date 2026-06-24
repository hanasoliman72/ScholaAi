using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Services.Base;
using ScholaAi.DTOs.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task DebitWalletAsync(string userId, decimal amount)
        {
            var wallet = await GetOrCreateWalletAsync(userId);
            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient wallet balance.");

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.updateAsync(wallet);              
            await _walletRepository.SaveChangesAsync();
        }

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

        public async Task RecordTopupAsync(string userId, decimal amount)
        {
            var transaction = new Transaction
            {
                FromWalletId = null,
                ToWalletId = userId,
                SessionId = null,
                Amount = amount,
                PlatformFee = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _walletRepository.AddTransactionAsync(transaction);
            await _walletRepository.SaveChangesAsync();
        }

        public async Task<List<UserTransactionDto>> GetTransactionsByUserIdAsync(string userId)
        {
            var dbTransactions = await _walletRepository.GetTransactionsByUserIdAsync(userId);
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            var currentBalance = wallet?.Balance ?? 0;

            var result = new List<UserTransactionDto>();
            var runningBalance = currentBalance;

            foreach (var t in dbTransactions)
            {
                var type = t.ToWalletId == userId ? "credit" : "debit";
                
                string description = "Wallet top-up (Stripe)";
                if (t.Session != null)
                {
                    if (type == "debit")
                    {
                        var teacherUser = t.Session.Teacher?.ApplicationUser;
                        var teacherName = teacherUser != null ? $"{teacherUser.FirstName} {teacherUser.LastName}" : "Teacher";
                        description = $"Session with {teacherName}";
                    }
                    else
                    {
                        var studentUser = t.Session.Student?.ApplicationUser;
                        var studentName = studentUser != null ? $"{studentUser.FirstName} {studentUser.LastName}" : "Student";
                        description = $"Session payout from {studentName}";
                    }
                }

                result.Add(new UserTransactionDto
                {
                    Id = t.TransactionId,
                    Type = type,
                    Amount = t.Amount,
                    Description = description,
                    Date = t.CreatedAt.ToString("MMM d, yyyy"),
                    Balance = runningBalance
                });

                if (type == "credit")
                {
                    runningBalance -= t.Amount;
                }
                else
                {
                    runningBalance += t.Amount;
                }
            }

            return result;
        }
    }
}
