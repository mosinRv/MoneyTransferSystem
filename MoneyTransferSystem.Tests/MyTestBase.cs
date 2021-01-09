using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using MoneyTransferSystem.Database;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    [TestFixture]
    public class MyTestBase
    {
        protected WebAppTestEnvironment Env { get; set; }
        protected HttpClient UserClient { get; set; }
        protected HttpClient AdminClient { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            Env = new WebAppTestEnvironment();
            Env.Start();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Env.Dispose();
            UserClient?.Dispose();
            AdminClient?.Dispose();
        }

        [SetUp]
        public async Task Prepare()
        {
            await Env.Prepare();
            UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
            AdminClient = CreateAuthorizedClientAsync("TestAdmin", "123").GetAwaiter().GetResult();
            
        }
        

        private async Task<HttpClient> CreateAuthorizedClientAsync(string login, string pass)
        {
            var client = Env.WebAppHost.GetClient();
            var res = await client.SignInAsync(login,pass);
            client.DefaultRequestHeaders.Add(HeaderNames.Cookie, res.Headers.GetValues(HeaderNames.SetCookie));
            return client;
        }
    }
}