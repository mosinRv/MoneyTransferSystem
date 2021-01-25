using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class CurrencyTest : MyTestBase
    {
        private int _usdId, _rubId, _newCurrId;
        [OneTimeSetUp]
        public override async Task Init()
        {
            Env = new WebAppTestEnvironment();
            await Env.Start();
            
            Dictionary<string, int> currenciesId = await Env.FindOrCreateCurrencies();
            _usdId = currenciesId["USD"];
            _rubId = currenciesId["RUB"];
            
            AdminClient = CreateAuthorizedClientAsync("TestAdmin", "123").GetAwaiter().GetResult();
            UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
        }

        public override Task Prepare()
        {
            return Task.CompletedTask;
        }


        [Test]
        public async Task GetCurrency()
        {
            // Arrange
            // Act
            HttpResponseMessage response =UserClient.GetCurrency().GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = JsonConvert.DeserializeObject<IEnumerable<Currency>>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
        }
        
        [Test]
        public async Task GetCurrencyById()
        {
            // Arrange
            int currId = _usdId;

            // Act
            HttpResponseMessage response =UserClient.GetCurrencyById(currId).GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = JsonConvert.DeserializeObject<Currency>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(_usdId,content.Id);
        }
        
        [Test, Order(1)]
        public async Task AddNewCurrency()
        {
            // Arrange
            var curr = new Currency {Name = "Test", CharCode = "TEST"};
            
            // Act
            HttpResponseMessage response = AdminClient.AddNewCurrency(curr).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var content = JsonConvert.DeserializeObject<Currency>(await response.Content.ReadAsStringAsync());
            _newCurrId = content.Id;
            Assert.AreEqual(curr.Name,content.Name);
        }
        [Test, Order(2)]
        public void ChangeCurrency()
        {
            // Arrange
            int id = _newCurrId;
            var curr = new Currency {Name = "Changed", CharCode = "TEST"};

            // Act
            HttpResponseMessage response = AdminClient.ChangeCurrency(id, curr).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(3)]
        public void DeleteCurrency()
        {
            // Arrange
            int id = _newCurrId;

            // Act
            HttpResponseMessage response = AdminClient.DeleteCurrency(id).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}