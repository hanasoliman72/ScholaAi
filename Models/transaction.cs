using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        // FK to Wallets (ApplicationUserId is string)
        [Required]
        public string FromWalletId { get; set; }

        [Required]
        public string ToWalletId { get; set; }

        // FK to Session
        [Required]
        public int SessionId { get; set; }

        [Precision(18, 4)]
        public decimal Amount { get; set; } = 0;

        [Precision(18, 4)]
        public decimal PlatformFee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(FromWalletId))]
        public Wallet FromWallet { get; set; }

        [ForeignKey(nameof(ToWalletId))]
        public Wallet ToWallet { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }
    }
}
