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
        public TransferStatus Status { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }

    public enum TransferType
    {
        TransferFrom,
        TransferTo,
        Deposit,
        Withdrawal
    }

    public enum TransferStatus
    {
        Approved,
        Declined,
        Pending
    }
}