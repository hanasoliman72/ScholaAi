using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace ScholaAi.Models
{
    public class wallet
    {
        [Key]
        public int walletId { get; set; }
        public int userId { get; set; }
        [Precision(18, 4)]
        public decimal balance { get; set; } = 0;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

        public user? user { get; set; }

        public ICollection<transaction> transactionsFrom { get; set; } = new List<transaction>();
        public ICollection<transaction> transactionsTo { get; set; } = new List<transaction>();
    }
}
