using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Tests
{
    public class WebAppTestEnvironment : IDisposable
    {
        private MyDbContext _context;
        public WebAppTestHost WebAppHost { get; }
        public object TestData;

        public WebAppTestEnvironment()
        {
            WebAppHost = new WebAppTestHost();
            
        }

        public void Start()
        {
            WebAppHost.Start();
            _context = WebAppHost.Services.GetRequiredService<MyDbContext>();
        }

        public async Task Prepare()
        {
            await PrepareUsers();
            await RemoveTestCreatedUser();

        }

        public void PrepareCommissions()
        {
            //TODO Add commissions into db
        }
        public void Dispose()
        {
            WebAppHost?.Dispose();
        }
        
        private async Task PrepareUsers()
        {
            Currency usd = await _context.Currencies.FirstOrDefaultAsync(c => c.CharCode == "USD")
                           ?? new Currency{Name="Dollar", CharCode = "USD"};
            Currency rub = await _context.Currencies.FirstOrDefaultAsync(c => c.CharCode == "RUB")
                           ?? new Currency{Name = "Ruble", CharCode = "RUB"};
            usd.MaxTransferSize = 150;
            rub.MaxTransferSize = 10000;
            
            var user = new User
            {
                Login = "TestUser", Pass = "123", isAdmin = false,
                Accounts = new List<Account>
                {
                    new Account {Money = 1000, Currency = usd}
                }
            };
            User oldUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == "TestUser");
            if (oldUser == null)
                _context.Users.Add(user);
            else
            {
                oldUser.Accounts = user.Accounts;
                oldUser.isAdmin = user.isAdmin;
                oldUser.Pass = user.Pass;
                _context.Users.Update(oldUser);
            }
            
            var user2 = new User
            {
                Login = "TestUser2", Pass = "123", isAdmin = false,
                Accounts = new List<Account>
                {
                    new Account {Money = 1000, Currency = usd},
                    new Account {Money = 1000, Currency = rub}
                }
            };
            User oldUser2 = await _context.Users.FirstOrDefaultAsync(u => u.Login == "TestUser");
            if (oldUser2 == null)
                _context.Users.Add(user2);
            else
            {
                oldUser2.Accounts = user2.Accounts;
                oldUser2.isAdmin = user2.isAdmin;
                oldUser2.Pass = user2.Pass;
                _context.Users.Update(oldUser2);
            }
            
            var admin = new User {Login = "TestAdmin", Pass = "123", isAdmin = true};
            User oldAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Login == "TestAdmin");
            if (oldAdmin == null)
                _context.Users.Add(admin);
            else
            {
                oldAdmin.Pass = admin.Pass;
                oldAdmin.isAdmin = admin.isAdmin;
                _context.Users.Update(oldAdmin);
            }
            
            await _context.SaveChangesAsync();
        }

        private async Task RemoveTestCreatedUser()
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == "ValidUser");
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        

    }
}