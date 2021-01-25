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
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class AccountTest : MyTestBase
    {
        public override async Task Init()
        {
            await base.Init();
            UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
        }
        

        [Test]
        public void GetTestUserInfo()
        {
            // Arrange
            // Act
            HttpResponseMessage response = UserClient.GetCurrentUserInfo().GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Content);
            var type = (response.Content.GetType());
            
        }

        [Test]
        public async Task WithdrawSuccessfully()
        {
            // Arrange
            const decimal money = 10;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money - money;

            // Act
            HttpResponseMessage response = UserClient.WithdrawMoney(accId, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == accId).Select(a => a.Money).FirstAsync();
            
            Assert.AreEqual(moneyShouldBeAfter, moneyAfter);
        }
        
        [Test]
        public async Task WithdrawBiggerThanOnAccount()
        {
            // Arrange
            decimal money = TestUser1.Accounts.First().Money + 1;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money;

            // Act
            HttpResponseMessage response = UserClient.WithdrawMoney(accId, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == accId).Select(a => a.Money).FirstAsync();
            
            Assert.AreEqual(moneyShouldBeAfter, moneyAfter);
        }

        
        [Test]
        public async Task DepositSuccessfully()
        {
            // Arrange
            const decimal money = 10;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money + money;

            // Act
            HttpResponseMessage response = UserClient.DepositMoney(accId, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == accId).Select(a => a.Money).FirstAsync();

            Assert.AreEqual(moneyShouldBeAfter, moneyAfter);
        }

        [Test]
        public async Task TransferSuccessfully_SameCurrency()
        {
            // Arrange
            const decimal money = 10;
            var accTo = TestUser2.Accounts.First();
            var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId==accTo.CurrencyId);

            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
            decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();
            

            Assert.AreEqual(accFrom.Money - money, moneyFromAfter);
            Assert.AreEqual(accTo.Money + money, moneyToAfter);
        }

        [Test]
        public async Task TransferSuccessfully_DifferentCurrency()
        {
            // Arrange
            const decimal money = 10;
            var accTo = TestUser2.Accounts.First();
            var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId!=accTo.CurrencyId);

            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
            decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();

            Assert.AreEqual(accFrom.Money - money, moneyFromAfter);
            Assert.IsTrue(accTo.Money < moneyToAfter);
        }
        
        
    }
}