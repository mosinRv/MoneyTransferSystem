

namespace MoneyTransferSystem.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public decimal Money { get; set; }
    }

    public class AccountDetailDto : AccountDto
    {
        public int UserId { get; set; }
        public string User { get; set; }
    }
}