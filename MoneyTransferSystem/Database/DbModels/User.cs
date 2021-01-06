using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;

namespace MoneyTransferSystem.Database.DbModels
{
    public class User
    {
        public int Id { get; set; }
        [Required, MinLength(3)]
        public string Login { get; set; }
        [Required, MinLength(3)]
        public string Pass { get; set; }
        [Required]
        public bool isAdmin { get; set; }
        
        public ICollection<Account>Accounts { get; set; }
    }
}