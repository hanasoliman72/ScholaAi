using ScholaAi.Models;

namespace ScholaAi.Repositories.Base
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(string userId);
        Task AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetTransactionsByUserIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
