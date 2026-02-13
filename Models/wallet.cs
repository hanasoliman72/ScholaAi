using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ScholaAi.Models
{
    public class Wallet
    {
        [Key]
        public string ApplicationUserId { get; set; }

        [Precision(18, 4)]
        public decimal Balance { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation to App
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        // Transactions
        public ICollection<Transaction> TransactionsFrom { get; set; } = new List<Transaction>();
        public ICollection<Transaction> TransactionsTo { get; set; } = new List<Transaction>();
    }
}