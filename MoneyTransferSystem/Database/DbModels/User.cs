using System.Collections.Generic;

namespace MoneyTransferSystem.Database.DbModels
{
    public class User
    {
        public int Id { get; set; }
        // Unique Index
        public string Login { get; set; }
        public string Pass { get; set; }
        public bool isAdmin { get; set; }
        
        public ICollection<Account>Accounts { get; set; }
    }
}