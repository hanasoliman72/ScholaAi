using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScholaAi.Models
{
    public class transaction
    {
        [Key]
        public int transactionId { get; set; }
        public int fromWalletId { get; set; }
        public int toWalletId { get; set; }
        public int sessionId { get; set; }
        [Precision(18, 4)]
        public decimal amount { get; set; } = 0;
        [Precision(18, 4)]
        public decimal platformFee { get; set; }
        public DateTime createdAt { get; set; }= DateTime.UtcNow;

        public wallet? fromWallet { get; set; }
        public wallet? toWallet { get; set; }
        [ForeignKey(nameof(sessionId))]
        public session? session { get; set; }
        //public ICollection<sessionRequest> sessionRequests { get; set; } = new List<sessionRequest>();
    }
}
