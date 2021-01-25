using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Tests
{
    public class WebAppTestEnvironment : IDisposable
    {
        // private MyDbContext _context;
        public WebAppTestHost WebAppHost { get; }

        private const string User1Name = "TestUser";
        private const string User2Name = "TestUser2";
        private const string AdminName = "TestAdmin";
        

        public WebAppTestEnvironment()
        {
            WebAppHost = new WebAppTestHost();
            
        }

        public async Task Start()
        {
            WebAppHost.Start();
            
            await CreateUsers();
        }

        public void Dispose()
        {
            WebAppHost?.Dispose();
        }
        
        private async Task CreateUsers()
        {
            Dictionary<string, int> currenciesId = await FindOrCreateCurrencies();
            int usdId = currenciesId["USD"];
            int rubId = currenciesId["RUB"];
            
            var testUser = new User
            {
                Login = User1Name, Pass = "123", isAdmin = false,
                Accounts = new List<Account>
                {
                    new Account {Money = 1000, CurrencyId = usdId},
                    new Account {Money = 1000, CurrencyId = rubId}
                }
            };
            var testUser2 = new User
            {
                Login = User2Name, Pass = "123", isAdmin = false,
                Accounts = new List<Account>
                {
                    new Account {Money = 1000, CurrencyId = usdId},
                }
            };
            var testAdmin = new User {Login = AdminName, Pass = "123", isAdmin = true};

            var context= WebAppHost.Services.GetRequiredService<MyDbContext>();
            context.Users.AddRange(testAdmin, testUser, testUser2);
            await context.SaveChangesAsync();
        }
        public async Task<Dictionary<string, int>> FindOrCreateCurrencies()
        {
            // If currency dont exist, there will be created test currencies, and after the tests they will be deleted
            var context= WebAppHost.Services.GetRequiredService<MyDbContext>();
            Currency usd = await context.Currencies.FirstOrDefaultAsync(c => c.CharCode == "USD");
            Currency rub = await context.Currencies.FirstOrDefaultAsync(c => c.CharCode == "RUB");
            
            if (usd == null || rub == null)
            {
                EntityEntry<Currency> tmpUsd = null, tmpRub = null;
                if (usd == null)
                    tmpUsd = context.Currencies.Add(new Currency
                        {Name = "TestDollar", CharCode = "USD", MaxTransferSize = 150});
                if (rub == null)
                    tmpRub = context.Currencies.Add(new Currency
                        {Name = "TestRuble", CharCode = "RUB", MaxTransferSize = 10000});
                await context.SaveChangesAsync();
                usd ??= tmpUsd.Entity;
                rub ??= tmpRub.Entity;
            }
            
            return new Dictionary<string, int>(){{"USD", usd.Id}, {"RUB", rub.Id}};
        }

        public async Task DeleteTestUsers()
        {
            var context= WebAppHost.Services.GetRequiredService<MyDbContext>();
            Currency usdTest = await context.Currencies.FirstOrDefaultAsync(c => c.Name == "TestDollar");
            Currency rubTest = await context.Currencies.FirstOrDefaultAsync(c => c.Name == "TestRuble");
            User testUser = await context.Users.FirstOrDefaultAsync(u => u.Login == User1Name);
            User testUser2 = await context.Users.FirstOrDefaultAsync(u => u.Login == User2Name);
            User testAdmin = await context.Users.FirstOrDefaultAsync(u => u.Login == AdminName);
            
            context.Users.RemoveRange(testUser, testUser2, testAdmin);
            if (usdTest != null) context.Currencies.Remove(usdTest);
            if (rubTest != null) context.Currencies.Remove(rubTest);
            await context.SaveChangesAsync();
        }



    }
}