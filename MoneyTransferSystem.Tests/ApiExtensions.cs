using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MoneyTransferSystem.Database.DbModels;
using Newtonsoft.Json;

namespace MoneyTransferSystem.Tests
{
    public static class ApiExtensions
    {
        #region Login

        public static async Task<HttpResponseMessage> SignInAsync(this HttpClient client, string login, string pass) =>
            await client.PostAsync($"api/login/sign-in/?Login={login}&Pass={pass}", null);
        public static async Task<HttpResponseMessage> SignOutAsync(this HttpClient client) =>
            await client.PostAsync($"api/login/sign-out", null);
        
        #endregion

        
        #region User

        public static async Task<HttpResponseMessage> GetUsers(this HttpClient client) => 
            await client.GetAsync("api/user");
        public static async Task<HttpResponseMessage> GetUser(this HttpClient client, int id) => 
            await client.GetAsync($"api/user/{id}");
        
        public static async Task<HttpResponseMessage> CreateUser(this HttpClient client, User newUser)
        {
            string userJson = JsonConvert.SerializeObject(newUser);
            return await client.PostAsync($"api/user", new StringContent(userJson, Encoding.UTF8, "application/json"));
        }

        #endregion


        #region Account

        // User role
        public static async Task<HttpResponseMessage> GetCurrentUserInfo(this HttpClient client) => 
            await client.GetAsync("api/account");
        
        public static async Task<HttpResponseMessage> WithdrawMoney(this HttpClient client, int accId, decimal money)
        {
            return await client.PostAsync($"api/account/withdraw/?accId={accId}&money={money}", null);
        }
        
        public static async Task<HttpResponseMessage> DepositMoney(this HttpClient client, int accId, decimal money)
        {
            return await client.PostAsync($"api/account/deposit/?accId={accId}&money={money}", null);
        }
        
        public static async Task<HttpResponseMessage> TransferMoney(this HttpClient client, int fromId, int toId, decimal money)
        {
            return await client.PostAsync($"api/account/transfer/?fromId={fromId}&toId={toId}&money={decimal.Round(money)}", null);
        }
        
        // Admin role
        public static async Task<HttpResponseMessage> GetAccounts(this HttpClient client) => 
            await client.GetAsync($"api/admin/account");
        
        public static async Task<HttpResponseMessage> GetAccountById(this HttpClient client, int id) => 
            await client.GetAsync($"api/admin/account/{id}");
        
        public static async Task<HttpResponseMessage> CreateAccount(this HttpClient client, Account newAccount)
        {
            string accJson = JsonConvert.SerializeObject(newAccount);
            return await client.PostAsync($"api/admin/account", new StringContent(accJson, Encoding.UTF8, "application/json"));
        }

        public static async Task<HttpResponseMessage> ConfirmTransfer(this HttpClient client, int id, bool confirmed)
        {
            return await client.PostAsync($"api/admin/account/transfer-confirmation/?id={id}&isConfirmed={confirmed}", null);
        }

        #endregion


        #region Currency

        public static async Task<HttpResponseMessage> GetCurrency(this HttpClient client)
        {
            return await client.GetAsync("api/currency");
        }
        
        public static async Task<HttpResponseMessage> GetCurrencyById(this HttpClient client, int id)
        {
            return await client.GetAsync($"api/currency/{id}");
        }
        
        public static async Task<HttpResponseMessage> AddNewCurrency(this HttpClient client, Currency currency)
        {
            string currJson = JsonConvert.SerializeObject(currency);
            return await client.PostAsync("api/currency", new StringContent(currJson, Encoding.UTF8, "application/json"));
        }

        public static async Task<HttpResponseMessage> ChangeCurrency(this HttpClient client, int id, Currency currency)
        {
            string currJson = JsonConvert.SerializeObject(currency);
            return await client.PutAsync($"api/currency/?id={id}", new StringContent(currJson, Encoding.UTF8, "application/json"));
        }
        public static async Task<HttpResponseMessage> DeleteCurrency(this HttpClient client, int id)
        {
            return await client.DeleteAsync($"api/currency/?id={id}");
        } 
        #endregion

        #region Commission

        public static async Task<HttpResponseMessage> GetGlobalCommission(this HttpClient client)
        {
            return await client.GetAsync("api/commission/global");
        }
        
        public static async Task<HttpResponseMessage> GetCustomCommission(this HttpClient client)
        {
            return await client.GetAsync("api/commission/custom");
        }
        
        public static async Task<HttpResponseMessage> CreateGlobalCommission(this HttpClient client, GlobalCommission commission)
        {
            string currJson = JsonConvert.SerializeObject(commission);
            return await client.PostAsync("api/commission/global", new StringContent(currJson, Encoding.UTF8, "application/json"));
        }
        public static async Task<HttpResponseMessage> CreateCustomCommission(this HttpClient client, PersonalCommission commission)
        {
            string currJson = JsonConvert.SerializeObject(commission);
            return await client.PostAsync("api/commission/custom", new StringContent(currJson, Encoding.UTF8, "application/json"));
        }

        #endregion
    }
}