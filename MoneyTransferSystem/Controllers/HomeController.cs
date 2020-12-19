using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Controllers
{
    public class HomeController : Controller
    {
        MyDbContext db;
        public HomeController(MyDbContext context)
        {
            db = context;
        }
        // GET
        public IActionResult Index()
        {
            // Dictionary<string, List<string[]>> usersInfo = new Dictionary<string, List<string[]>>();
            //
            // var users = db.Users;
            // foreach (var user in users)
            // {
            //     
            //     foreach (var acc in user.Accounts)
            //     {
            //         
            //     }   
            //     usersInfo.Add(user.Login,);
            // }
            // users.First().Accounts
            // var accounts = db.Accounts;
            return View();
        }
        [Authorize(Roles = "User")]
        public async Task<string> TransferUserMoney(int fromAccId, int toAccId, decimal money,
            [FromServices] ICurrencyConverter currencyConverter)
        {
            ClaimsPrincipal principal = HttpContext.User;
            Claim loginClaim = principal.FindFirst("login");
            string login = loginClaim?.Value;
            //check if account belongs to current user
            var userId = db.Accounts.FirstOrDefault(u => u.Id == fromAccId)?.UserId ?? -1;
            if (userId!=-1 && db.Users.FirstOrDefault(u=>u.Id==userId)?.Login == login)
                return await TransferMoney(fromAccId, toAccId, money, currencyConverter);
            return "Insufficient rights or incorrect value of Id account parameter";
        }

        [Authorize(Roles = "Admin")]
        public async Task<string>  TransferMoney(int fromAccId, int toAccId, decimal money, [FromServices] ICurrencyConverter currencyConverter)
        {
            Account accFrom = db.Accounts.First(acc => acc.Id == fromAccId);
            Account accTo = db.Accounts.First(acc => acc.Id == toAccId);

            if (accFrom.Money < money)
                throw new Exception("Account dont have enough money to make a transfer");

            accFrom.Money -= money;
            
            //Find global commission information
            var glRule = db.GlobalMoneyRules.FirstOrDefault(r => 
                r.CurrencyId == accFrom.CurrencyId 
                && r.Min<=money 
                && r.Max>=money);
            //Find personal user commission information
            var perRule = db.MoneyRules.FirstOrDefault(r =>
                r.CurrencyId == accFrom.CurrencyId
                && r.Min <= money
                && r.Max >= money);
            decimal moneyToReceive = money;
            //var role = perRule ?? glRule; //mb add parent for both classes?
            if (perRule != null)
            {
                decimal commission = perRule.isComissionFixed ? perRule.Comission : moneyToReceive * perRule.Comission;
                moneyToReceive -= commission;
            }
            else if (glRule != null)
            {
                decimal commission = glRule.isComissionFixed ? glRule.Comission : moneyToReceive * glRule.Comission;
                moneyToReceive -= commission;
            }
            if (moneyToReceive < 0) moneyToReceive = 0;
            
            moneyToReceive = currencyConverter.ConvertMoney(moneyToReceive, accFrom.Currency, accTo.Currency);
            accTo.Money += moneyToReceive;

            await db.SaveChangesAsync();
            return "The transfer of money has been made";
        }
        

        #region Put/Take money from Account

        public async Task<string> PutMoneyOnAccount(int accountId, decimal money)
        {
            //Подразумевается, что аккаунт пополняется в соответствующей аккаунту валюте
            var acc = db.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (money < 0 || acc == null) 
                return "Incorrectly entered data to replenish the account";
            
            db.Accounts.First(a => a.Id == accountId).Money += money;
            await db.SaveChangesAsync();
            return "Account is replenished";
        }
        public async Task<string> TakeMoneyFromAccount(int accountId, decimal money)
        {
            var acc = db.Accounts.First(a => a.Id == accountId);
            if (money < 0 || !(acc?.Money >= money)) 
                return "Incorrectly entered data for withdrawal from the account";
            
            acc.Money -= money;
            await db.SaveChangesAsync();
            return "Money withdrawn from the account";
        }
        #endregion
    }
}