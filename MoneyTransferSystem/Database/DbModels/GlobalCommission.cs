using System.ComponentModel.DataAnnotations;

namespace MoneyTransferSystem.Database.DbModels
{
    public class GlobalCommission
    // These rules apply to all users
    {
        public int Id { get; set; }
        [Required]
        public decimal Min { get; set; }
        [Required]
        public decimal Max { get; set; }
        [Required]
        public decimal Commission { get; set; }
        [Required]
        public bool isCommissionFixed { get; set; }
        
        [Required]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
