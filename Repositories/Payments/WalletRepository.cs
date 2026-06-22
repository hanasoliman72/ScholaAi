using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;

namespace ScholaAi.Repositories.Payments
{
    public class WalletRepository : genericRepository<Wallet>, IWalletRepository
    {
        private readonly DBcontext _context;
        public WalletRepository(DBcontext context) : base(context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<Wallet?> GetByUserIdAsync(string userId)
        {
           return await  _context.Wallets
                .FirstOrDefaultAsync( w => w.ApplicationUserId == userId );
        }

        public async Task<List<Transaction>> GetTransactionsByUserIdAsync(string userId)
        {
            return await _context.Transactions
                .Include(t => t.Session)
                    .ThenInclude(s => s.Teacher)
                        .ThenInclude(te => te.ApplicationUser)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Student)
                        .ThenInclude(st => st.ApplicationUser)
                .Where(t => t.FromWalletId == userId || t.ToWalletId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
