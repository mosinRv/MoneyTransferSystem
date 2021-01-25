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
    public class AccountWithCommissionTest : MyTestBase
    {
        private PersonalCommission _commission;
        public override async Task Init()
        {
            await base.Init();
            UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
            
            // Create custom commission
            _commission = new PersonalCommission
            {
                CurrencyId = TestUser1.Accounts.First().CurrencyId,
                Commission = (decimal) 0.5, // 50%
                isCommissionFixed = false,
                Min = 10,
                Max = 20,
                UserId = TestUser1.Id
            };
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            
            context.MoneyRules.Add(_commission);
            await context.SaveChangesAsync();
        }

        [Test]
        public async Task DepositSuccessfully()
        {
            // Arrange
            decimal money = _commission.Min / _commission.Commission + 1;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money + money*(1-_commission.Commission);

            // Act
            HttpResponseMessage response = UserClient.DepositMoney(accId, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == accId).Select(a => a.Money).FirstAsync();
            
            Assert.AreEqual(moneyShouldBeAfter, moneyAfter);
        }
        
        [Test]
        public async Task Deposit_MinCommission()
        {
            // Arrange
            decimal money = _commission.Min / _commission.Commission - 1;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money + money - _commission.Min;

            // Act
            HttpResponseMessage response = UserClient.DepositMoney(accId, money).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyAfter = await context.Accounts.Where(a => a.Id == accId).Select(a => a.Money).FirstAsync();
            
            Assert.AreEqual(moneyShouldBeAfter, moneyAfter);
        }
        
        [Test]
        public async Task Deposit_MaxCommission()
        {
            // Arrange
            decimal money = _commission.Max / _commission.Commission + 1;
            int accId = TestUser1.Accounts.First().Id;
            decimal moneyShouldBeAfter = TestUser1.Accounts.First().Money + money - _commission.Max;

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
            decimal money = _commission.Min / _commission.Commission * (1.0m - _commission.Commission) + 1;
            var accTo = TestUser2.Accounts.First();
            var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId==accTo.CurrencyId);
            decimal commission = decimal.Round(money)/(1.0m- _commission.Commission)*_commission.Commission;
        
            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
            decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();


            Assert.AreEqual(accFrom.Money - money - commission, moneyFromAfter);
            Assert.AreEqual(accTo.Money + money, moneyToAfter);
        }
        
        [Test]
        public async Task Transfer_MaxCommission_SameCurrency()
        {
            // Arrange
            decimal money = _commission.Max / _commission.Commission * (1.0m - _commission.Commission)+1;
            var accTo = TestUser2.Accounts.First();
            var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId==accTo.CurrencyId);
            decimal commission = _commission.Max;
        
            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
            decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();


            Assert.AreEqual(accFrom.Money - money - commission, moneyFromAfter);
            Assert.AreEqual(accTo.Money + money, moneyToAfter);
        }
        
        [Test]
        public async Task Transfer_MinCommission_SameCurrency()
        {
            // Arrange
            decimal money = _commission.Min / _commission.Commission * (1.0m - _commission.Commission);
            var accTo = TestUser2.Accounts.First();
            var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId==accTo.CurrencyId);
            decimal commission =  _commission.Min;
        
            // Act
            HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();
        
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
            decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();


            Assert.AreEqual(accFrom.Money - money - commission, moneyFromAfter);
            Assert.AreEqual(accTo.Money + money, moneyToAfter);
        }
        // [Test]
        // public async Task TransferSuccessfully_DifferentCurrency()
        // {
        //     // Arrange
        //     const decimal money = 10;
        //     var accTo = TestUser2.Accounts.First();
        //     var accFrom = TestUser1.Accounts.First(a=>a.CurrencyId!=accTo.CurrencyId);
        //
        //     // Act
        //     HttpResponseMessage response = UserClient.TransferMoney(accFrom.Id,accTo.Id, money).GetAwaiter().GetResult();
        //
        //     // Assert
        //     Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //
        //     var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
        //     decimal moneyFromAfter = await context.Accounts.Where(a => a.Id == accFrom.Id).Select(a => a.Money).FirstAsync();
        //     decimal moneyToAfter = await context.Accounts.Where(a => a.Id == accTo.Id).Select(a => a.Money).FirstAsync();
        //
        //     Assert.AreEqual(accFrom.Money - money, moneyFromAfter);
        //     Assert.IsTrue(accTo.Money < moneyToAfter);
        // }
        
        
    }
}