namespace MoneyTransferSystem.Database.DbModels
{
    public class Account
    {
        public int Id { get; set; }
        public decimal Money { get; set; }
        
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}