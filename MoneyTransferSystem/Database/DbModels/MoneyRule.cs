namespace MoneyTransferSystem.Database.DbModels
{
    public class MoneyRule
    {
        public int Id { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Comission { get; set; }
        public bool isComissionFixed { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}