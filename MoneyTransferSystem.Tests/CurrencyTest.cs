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
    public class CurrencyTest: MyTestBase
    {
        [Test]
        public void GetCurrency()
        {
            // Arrange
            // Act
            HttpResponseMessage response =AdminClient.GetCurrency().GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Test]
        public async Task GetCurrencyById()
        {
            // Arrange
            int currId;
            await using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                currId=await context.Currencies.Select(c => c.Id).FirstAsync();
            }

            // Act
            HttpResponseMessage response =AdminClient.GetCurrencyById(currId).GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Test]
        public async Task AddNewCurrency()
        {
            // Arrange
            var curr = new Currency {Name = "Test", CharCode = "TEST"};
            await using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                Currency oldCurr = await context.Currencies.FirstOrDefaultAsync(c => c.CharCode == curr.CharCode);
                if (oldCurr != null)
                    context.Currencies.Remove(oldCurr);
                await context.SaveChangesAsync();
            }
            // Act
            HttpResponseMessage response = AdminClient.AddNewCurrency(curr).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
        [Test]
        public async Task ChangeCurrency()
        {
            // Arrange
            var curr = new Currency {Name = "Test", CharCode = "TEST"};
            await using (var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>())
            {
                Currency oldCurr = await context.Currencies.FirstOrDefaultAsync(c => c.CharCode == curr.CharCode);
                if (oldCurr == null)
                {
                    var tmp = context.Currencies.Add(curr);
                    await context.SaveChangesAsync();
                    curr.Id = tmp.Entity.Id;
                }
                else
                    curr.Id = oldCurr.Id;
            }
            curr.Name = "Changed";
            
            // Act
            HttpResponseMessage response = AdminClient.ChangeCurrency(curr.Id, curr).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}