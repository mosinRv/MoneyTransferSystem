using System.Collections.Generic;

namespace MoneyTransferSystem.Database.DbModels
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CharCode { get; set; }
        
        public ICollection<Account> Accounts { get; set; }
        public ICollection<MoneyRule> MoneyRules { get; set; }
        public ICollection<GlobalMoneyRule>GlobalMoneyRules { get; set; }
    }
}