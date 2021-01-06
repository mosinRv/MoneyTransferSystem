using System.ComponentModel.DataAnnotations;

namespace MoneyTransferSystem.Database.DbModels
{
    public class PersonalCommission
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
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}