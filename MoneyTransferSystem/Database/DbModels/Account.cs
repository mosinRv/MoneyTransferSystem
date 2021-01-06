using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace MoneyTransferSystem.Database.DbModels
{
    public class Account
    {
        public int Id { get; set; }
        [Required]
        public decimal Money { get; set; }
        
        [Required]
        public int CurrencyId { get; set; }
        [Required]
        public int UserId { get; set; }
        public Currency Currency { get; set; }
        public User User { get; set; }
        public ICollection<Transfer>Transfers { get; set; }
    }
}