﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoneyTransferSystem.Database.DbModels
{
    public class Currency
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CharCode { get; set; }
        // The maximum transfer size that does not require admin confirmation to perform
        public int MaxTransferSize { get; set; }
        
        public ICollection<Account> Accounts { get; set; }
        public ICollection<PersonalCommission> MoneyRules { get; set; }
        public ICollection<GlobalCommission>GlobalMoneyRules { get; set; }
    }
}