namespace MoneyTransferSystem.Database.DbModels
{
    public class GlobalMoneyRule
    // These rules apply to all users
    {
        public int Id { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Comission { get; set; }
        public bool isComissionFixed { get; set; }
        
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
