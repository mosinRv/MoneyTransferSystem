using System.ComponentModel.DataAnnotations;

namespace MoneyTransferSystem.Database.DbModels
{
    public class Transfer
    {
        public int Id { get; set; }
        [Required]
        public decimal Money { get; set; }
        [Required]
        public TransferType Type { get; set; }
        [Required]
        public bool isApproved { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }

    public enum TransferType
    {
        Transfer,
        Deposit,
        Withdrawal
    }
}