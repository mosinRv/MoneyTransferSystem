namespace MoneyTransferSystem.Models
{
    public class CommissionDto
    {
        public string Currency { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public string Commission { get; set; }
    }

    public class CommissionDetailDto : CommissionDto
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        
    }

    public class CustomCommissionDetailDto : CommissionDto
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
    }
}