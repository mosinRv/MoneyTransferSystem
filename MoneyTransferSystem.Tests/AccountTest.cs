using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class AccountTest : MyTestBase
    {
        [Test]
        public void GetCurrentUserInfo()
        {
            // Arrange
            // Act
            HttpResponseMessage response = UserClient.GetCurrentUserInfo().GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void GetAccounts()
        {
            // Arrange
            // Act
            HttpResponseMessage response = AdminClient.GetAccounts().GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void GetAccountById()
        {
            // Arrange
            int id;
            using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                id = context.Accounts.Select(a => a.Id).FirstAsync().GetAwaiter().GetResult();
            }

            // Act
            HttpResponseMessage response = AdminClient.GetAccountById(id).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void CreateAccount()
        {
            // Arrange
            int userId;
            int currencyId;
            using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                userId = context.Users.Where(u => u.Login == "TestUser").Select(u => u.Id)
                    .FirstAsync().GetAwaiter().GetResult();
                currencyId = context.Currencies.Select(c => c.Id)
                    .FirstAsync().GetAwaiter().GetResult();
            }

            Account acc = new Account
            {
                Money = 0,
                CurrencyId = currencyId,
                UserId = userId
            };
            // Act
            HttpResponseMessage response = AdminClient.CreateAccount(acc).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
        [Test]
        public void TakeOrPutMoney()
        {
            // Arrange
            int accId;
            decimal money = 10;
            using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                accId = context.Users.Where(u=>u.Login=="TestUser").Include(u=>u.Accounts)
                    .Select(u=>u.Accounts).FirstAsync().GetAwaiter().GetResult()
                    .Select(a=>a.Id).First();
            }

            // Act
            HttpResponseMessage response = UserClient.TakeOrPutMoney(accId,money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Test]
        public void TransferMoney()
        {
            // Arrange
            int accId1, accId2;
            decimal money = 10;
            using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                accId1 = context.Users.Where(u=>u.Login=="TestUser").Include(u=>u.Accounts)
                    .Select(u=>u.Accounts).FirstAsync().GetAwaiter().GetResult()
                    .Select(a=>a.Id).First();
                accId2 = context.Users.Where(u=>u.Login=="TestUser2").Include(u=>u.Accounts)
                    .Select(u=>u.Accounts).FirstAsync().GetAwaiter().GetResult()
                    .Select(a=>a.Id).First();
            }

            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accId1,accId2,money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Test]
        public async Task ConfirmTransfer()
        {
            // Arrange
            int transferId;
            using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                User testUser = await context.Users.Where(u => u.Login == "TestUser").Include(u => u.Accounts)
                    .FirstAsync();
                var transfer = new Transfer
                {
                    AccountId = testUser.Accounts.First().Id,
                    Money = 100,
                    Type = TransferType.Deposit
                };
                var tmp=context.Transfers.Add(transfer);
                await context.SaveChangesAsync();
                transferId = tmp.Entity.Id;
            }
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, true).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}