using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using NUnit.Framework;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MoneyTransferSystem.Tests
{
    [TestFixture]
    public class MyTestBase
    {
        protected User TestUser1;
        protected User TestUser2;
        protected WebAppTestEnvironment Env { get; set; }
        protected HttpClient UserClient { get; set; }
        protected HttpClient AdminClient { get; set; }

        [OneTimeSetUp]
        public virtual async Task Init()
        {
            Env = new WebAppTestEnvironment();
            await Env.Start();
            
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            TestUser1 = await context.Users.Where(u => u.Login == "TestUser")
                .Include(u => u.Accounts).FirstAsync();
            TestUser2 = await context.Users.Where(u => u.Login == "TestUser2")
                .Include(u => u.Accounts).FirstAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await Env.DeleteTestUsers();
            
            Env.Dispose();
            UserClient?.Dispose();
            AdminClient?.Dispose();
        }

        [SetUp]
        public virtual async Task Prepare()
        {
            await ResetUsersMoney();
            // await Env.Prepare();
            // UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
            // AdminClient = CreateAuthorizedClientAsync("TestAdmin", "123").GetAwaiter().GetResult();
        }
        private async Task ResetUsersMoney()
        {
            var context = Env.WebAppHost.Services.GetRequiredService<MyDbContext>();
            var accounts = await context.Accounts
                .Where(a => a.UserId == TestUser1.Id || a.UserId == TestUser2.Id).ToListAsync();
            foreach (var acc in accounts)
            {
                acc.Money = 1000;
                context.Accounts.Update(acc);
            }
            
            await context.SaveChangesAsync();
        }
        

        protected async Task<HttpClient> CreateAuthorizedClientAsync(string login, string pass)
        {
            var client = Env.WebAppHost.GetClient();
            var res = await client.SignInAsync(login,pass);
            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, res.Headers.GetValues(HeaderNames.SetCookie));
            return client;
        }
    }
}