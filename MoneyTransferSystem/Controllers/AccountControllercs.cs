using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Services;

namespace MoneyTransferSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;

        public AccountController(MyDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/account/my-accounts")]
        public async Task<JsonResult> GetCurrentUserInfo()
        {
            var userId = int.Parse(User.Identity.Name);
            var user = await _context.Users.Where(u => u.Id == userId).Include(u => u.Accounts)
                .ThenInclude(a => a.Currency).Select(u => new
                {
                    User = u.Login,
                    Role = u.isAdmin ? "Admin" : "User",
                    Accounts = u.Accounts.Select(a => new
                    {
                        Id = a.Id,
                        CurrencyId = a.CurrencyId,
                        Currency = a.Currency.CharCode,
                        Money = a.Money
                    })
                }).FirstOrDefaultAsync();
            
            return Json(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/account")]
        public async Task<JsonResult> GetAccounts(int count=10)
        {
            var accounts =  await _context.Accounts.Take(count).ToListAsync();
            return Json(accounts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/account/{id}", Name = "GetAccount")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (acc == null)
                return NotFound();
            return Json(acc);
        }
        [HttpPost("api/account"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount([FromBody]Account acc)
        {
            var a=_context.Accounts.Add(acc);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetAccount", new {id = a.Entity.Id}, a.Entity);
            // Created($"api/account/{a.Entity.Id}", a.Entity);
        }
        
        [HttpPost("api/account/money")]
        public async Task<IActionResult> TakeOrPutMoney(int accId, decimal money)
        {
            var userId = int.Parse(User.Identity.Name);
            var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            var acc = (role == "User")
                ? await _context.Accounts.Where(a => a.Id == accId && a.UserId == userId)
                    .Include(a => a.Currency).FirstOrDefaultAsync()
                : await _context.Accounts.Where(a => a.Id == accId)
                    .Include(a => a.Currency).FirstOrDefaultAsync();
            
            if (acc == null) return NotFound();
            if (money == 0) return BadRequest("value of money must be non-zero");
            if(money<0 && acc.Money<Math.Abs(money)) 
                return BadRequest("There are not enough funds on the account for withdrawal");

            bool? isApproved = Math.Abs(money) > acc.Currency.MaxTransferSize ? (bool?) null : true;
            if (isApproved == true)
            {
                if (money > 0) 
                    acc.Money += money;
                else 
                    acc.Money -= money;
                _context.Accounts.Update(acc);
            }
            var transfer= new Transfer {AccountId = accId, Money = Math.Abs(money),
                Type = money>0 ? TransferType.Deposit : TransferType.Withdrawal,
                isApproved = isApproved};
            
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("api/account/transfer")]
        public async Task<IActionResult> TransferMoney(int fromId, int toId, decimal money, [FromServices] ICurrencyConverter currencyConverter)
        {
            
            var userId = int.Parse(User.Identity.Name);
            Account accFrom = await _context.Accounts.Where(a => a.UserId == userId && a.Id == fromId)
                .Include(a => a.Currency).FirstOrDefaultAsync();

            if (accFrom == null) return BadRequest();
            if (accFrom.Money < money) return BadRequest("Account dont have enough money to make a transfer");
            
            var accTo = await _context.Accounts.Where(a => a.Id == toId)
                .Include(a => a.Currency).FirstOrDefaultAsync();
            if (accTo == null) return BadRequest();

            //Find global commission information
            var glRule = await _context.GlobalMoneyRules.FirstOrDefaultAsync(r => 
                r.CurrencyId == accFrom.CurrencyId 
                && r.Min<=money 
                && r.Max>=money);
            //Find custom user commission information
            var customRule = await _context.MoneyRules.FirstOrDefaultAsync(r =>
                r.CurrencyId == accFrom.CurrencyId
                && r.Min <= money
                && r.Max >= money);
            var moneyToTake = money;
            if (glRule != null)
            {
                decimal commission = glRule.isCommissionFixed ? glRule.Commission : moneyToTake * glRule.Commission;
                moneyToTake += commission;
            }
            if (customRule != null)
            {
                decimal commission =customRule.isCommissionFixed ? customRule.Commission : moneyToTake * customRule.Commission;
                moneyToTake += commission;
            }

            if (accFrom.Money < moneyToTake)
                return BadRequest(
                    "There are not enough funds on the account for the transfer, taking into consideration the fee");
            var curFrom = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == accFrom.CurrencyId);
            var curTo=await _context.Currencies.FirstOrDefaultAsync(c => c.Id == accTo.CurrencyId);
            decimal moneyToReceive = currencyConverter.ConvertMoney(money, curFrom, curTo);

            bool needConfirmToTake =
                accFrom.Currency.MaxTransferSize != 0 && moneyToTake > accFrom.Currency.MaxTransferSize;
            bool needConfirmToReceive =
                accTo.Currency.MaxTransferSize != 0 && moneyToReceive > accTo.Currency.MaxTransferSize;
            bool? isApproved = (needConfirmToReceive && needConfirmToTake) ? (bool?) null : true;
            
            // the sender's money will be frozen and will awaits confirmation
            accFrom.Money -= moneyToTake;
            if (isApproved==true)
                accTo.Money += moneyToReceive;
            
            var sending = new Transfer {AccountId = fromId, Money = -moneyToTake, Type = TransferType.Transfer, isApproved = isApproved};
            var delivery = new Transfer {AccountId = toId, Money = moneyToReceive, Type = TransferType.Transfer, isApproved = isApproved};
            
            _context.Accounts.UpdateRange(accFrom,accTo);
            await _context.Transfers.AddRangeAsync(sending,delivery);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("api/account/confirmation")]
        public async Task<IActionResult> ConfirmTransfer(int id, bool confirmed)
        {
            Transfer transfer = await _context.Transfers.Where(t => t.Id == id && t.isApproved == null)
                .Include(t => t.Account).FirstOrDefaultAsync();
            if (transfer == null) return NotFound();
            
            switch (transfer.Type)
            {
                case TransferType.Deposit when confirmed:
                    transfer.Account.Money += transfer.Money; 
                    break;
                case TransferType.Withdrawal when confirmed:
                    transfer.Account.Money -= transfer.Money; 
                    break;
                case TransferType.Transfer when confirmed:
                    transfer.Account.Money += transfer.Money; 
                    break;
                case TransferType.Transfer when transfer.Money<0:
                    // return frozen money
                    transfer.Money -= transfer.Money;
                    break;
            }
            transfer.isApproved = confirmed;
            return Ok();
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var valute =await _context.Currencies.FirstOrDefaultAsync(c => c.CharCode == "OOO");
            valute.Name = "asd";
            valute.MaxTransferSize = 12345;
            return Json(valute);
        }
    }
}