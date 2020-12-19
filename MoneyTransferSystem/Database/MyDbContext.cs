using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Database
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
            
        }
        
        public DbSet<User>Users { get; set; }
        public DbSet<Account>Accounts { get; set; }
        public DbSet<Currency>Currencies { get; set; }
        public DbSet<GlobalMoneyRule>GlobalMoneyRules { get; set; }
        public DbSet<MoneyRule>MoneyRules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Currency>(entityBuilder =>
            {
                entityBuilder.HasIndex(p => p.CharCode).IsUnique();
                entityBuilder.HasData(new Currency
                {
                    Id = 1,
                    Name = "USA dollar",
                    CharCode = "USD"
                });
            });
            builder.Entity<User>(entityBuilder =>
            {
                entityBuilder.HasIndex(p => p.Login).IsUnique();
                entityBuilder.HasData(new User
                {
                    Id = 1,
                    Login = "Adam",
                    Pass = "123",
                });
            });
        }
        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     #region Data
        //
        //     var user1 = new User {Login = "User_A", Pass = "123", isAdmin = false};
        //     var user2 = new User {Login = "User_B", Pass = "123", isAdmin = false};
        //     var user3 = new User {Login = "admin", Pass = "admin", isAdmin = true};
        //         
        //     var acc1 = new Account {Id = 1, Money = 1000, User = user1};
        //     var acc2 = new Account {Id=2, Money = 8000, User = user1};
        //     var acc3 = new Account {Id=3, Money = 10000, User = user2};
        //     
        //     var dollar = new Currency {Name = "USD", DollarEquivalent = 1};
        //     var ruble = new Currency {Name = "RUB", DollarEquivalent = 73.12};
        //     
        //     var gRulUSD1 = new GlobalMoneyRule
        //         {Min = 10, Max = 500, Comission = 0.1, isComissionFixed = false, Currency = dollar};
        //     var gRulUSD2 = new GlobalMoneyRule
        //         {Min = 500, Max = 2000, Comission = 100, isComissionFixed = true, Currency = dollar};
        //     var gRulRUB= new GlobalMoneyRule
        //         {Min = 500, Max = 2000, Comission = 0.5, isComissionFixed = false, Currency = ruble};
        //     
        //     var user1MR = new MoneyRule {Min = 100, Max = 10000, Comission = 90, isComissionFixed = true, User = user1};
        //     
        //
        //     #endregion
        //     
        //     builder.Entity<User>().HasData(user1,user2,user3);
        //     builder.Entity<Account>().HasData(acc1,acc2,acc3);
        //     builder.Entity<Currency>().HasData(dollar, ruble);
        //     builder.Entity<GlobalMoneyRule>().HasData(gRulRUB, gRulUSD1, gRulUSD2);
        //     builder.Entity<MoneyRule>().HasData(user1MR);
        // }
        
    }
}