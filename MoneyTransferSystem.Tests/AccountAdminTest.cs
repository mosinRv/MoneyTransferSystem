using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class AccountAdminTest : MyTestBase
    {
        public override async Task Init()
        {
            await base.Init();
            AdminClient = CreateAuthorizedClientAsync("TestAdmin", "123").GetAwaiter().GetResult();
        }

        [Test]
        public async Task GetAccounts()
        {
            // Arrange
            // Act
            HttpResponseMessage response = AdminClient.GetAccounts().GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = JsonConvert.DeserializeObject<IEnumerable<AccountDetailDto>>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
        }
        
        [Test]
        public async Task GetAccountById()
        {
            // Arrange
            int id = TestUser1.Accounts.First().Id;

            // Act
            HttpResponseMessage response = AdminClient.GetAccountById(id).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = JsonConvert.DeserializeObject<AccountDetailDto>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
        }
        
        [Test]
        public async Task CreateAccount()
        {
            // Arrange
            int userId = TestUser2.Id;
            int currencyId = TestUser1.Accounts.First(a => a.CurrencyId != TestUser2.Accounts.First().CurrencyId)
                .CurrencyId;

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
            var content = JsonConvert.DeserializeObject<Account>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
            Assert.AreEqual(userId, content.UserId);
        }
        
        [Test]
        public async Task ConfirmTransfer_Deposit()
        {
            // Arrange
            decimal money = 100;
            Account acc = TestUser1.Accounts.First();
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var tmp = context.Transfers.Add(
                new Transfer {AccountId = acc.Id, Money = money, Type = TransferType.Deposit});
            await context.SaveChangesAsync();
            int transferId = tmp.Entity.Id;
                
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, true).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == acc.Id).Select(a => a.Money).FirstAsync();
            Assert.AreEqual(acc.Money+money, moneyAfter);
        }
        
        [Test]
        public async Task ConfirmTransfer_Withdrawal()
        {
            // Arrange
            decimal money = 100;
            Account acc = TestUser1.Accounts.First();
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var tmp = context.Transfers.Add(
                new Transfer {AccountId = acc.Id, Money = money, Type = TransferType.Withdrawal});
            await context.SaveChangesAsync();
            int transferId = tmp.Entity.Id;
                
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, true).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == acc.Id).Select(a => a.Money).FirstAsync();
            Assert.AreEqual(acc.Money - money, moneyAfter);
        }
        
        [Test]
        public async Task ConfirmTransfer_TransferTo()
        {
            // Arrange
            decimal money = 100;
            Account acc = TestUser1.Accounts.First();
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var tmp = context.Transfers.Add(
                new Transfer {AccountId = acc.Id, Money = money, Type = TransferType.TransferTo});
            await context.SaveChangesAsync();
            int transferId = tmp.Entity.Id;
                
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, true).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == acc.Id).Select(a => a.Money).FirstAsync();
            Assert.AreEqual(acc.Money + money, moneyAfter);
        }
        
        [Test]
        public async Task ConfirmTransfer_TransferFrom_Сonfirmed()
        {
            // Arrange
            decimal money = 100;
            Account acc = TestUser1.Accounts.First();
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var tmp = context.Transfers.Add(
                new Transfer {AccountId = acc.Id, Money = -money, Type = TransferType.TransferFrom});
            await context.SaveChangesAsync();
            int transferId = tmp.Entity.Id;
                
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, true).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == acc.Id).Select(a => a.Money).FirstAsync();
            Assert.AreEqual(acc.Money, moneyAfter);
        }
        
        [Test]
        public async Task ConfirmTransfer_TransferFrom_Unconfirmed()
        {
            // Arrange
            decimal money = 100;
            Account acc = TestUser1.Accounts.First();
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var tmp = context.Transfers.Add(
                new Transfer {AccountId = acc.Id, Money = money, Type = TransferType.TransferFrom});
            await context.SaveChangesAsync();
            int transferId = tmp.Entity.Id;
                
        
            // Act
            HttpResponseMessage response = AdminClient.ConfirmTransfer(transferId, false).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == acc.Id).Select(a => a.Money).FirstAsync();
            Assert.AreEqual(acc.Money+money, moneyAfter);
        }
        
    }
}