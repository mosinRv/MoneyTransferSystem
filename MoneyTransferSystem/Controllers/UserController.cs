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
    [Authorize(Roles = "Admin, User")]
    public class UserController : Controller
    {
        private MyDbContext _db;

        public UserController(MyDbContext db)
        {
            this._db = db;
        }


        [HttpGet("api/user")]
        public async Task<JsonResult> GetCurrentUser()
        {
            var userId = int.Parse(User.Identity.Name);
            var user = await _db.Users.FirstAsync(u => u.Id == userId);
            user.Accounts = await _db.Accounts.Where(a => a.UserId == userId).ToListAsync();
            
            var result = new
            {
                User = user.Login,
                Accounts = user.Accounts.Select(async a => new
                {
                    Id = a.Id,
                    Currency = (await _db.Currencies.FirstAsync(c => c.Id == a.CurrencyId)).CharCode,
                    Money = a.Money
                })
            };
            return Json(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("api/user/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user== null)
                return NotFound();
            user.Accounts = await _db.Accounts.Where(a => a.UserId == id).ToListAsync();
            return Json(user);
        }
        [HttpPost("api/user"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(User user)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();
                var u = _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return CreatedAtRoute("GetUser", new {id = u.Entity.Id}, u.Entity);
            }
            catch
            {
                //TODO what for here next line
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return BadRequest();
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("api/user/account/{id}", Name = "GetAccount")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (acc== null)
                return NotFound();
            return Json(acc);
        }
        [HttpPost("api/user/account"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount(Account acc)
        {
            //TODO *add account verification
            var a=_db.Accounts.Add(acc);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetAccount", new {id = a.Entity.Id}, a.Entity);

        }
        [HttpPut("api/user/money/{accId}")]
        public async Task<IActionResult> TakeOrPutMoney(int accId, decimal money)
        {
            var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == accId);
            if (acc == null && money==0) return BadRequest();
            
            if (money > 0)
                acc.Money += money;
            else if(acc.Money>=money)
                acc.Money -= money;
            else
                return BadRequest("There are not enough funds on the account for withdrawal");
            

            _db.Accounts.Update(acc);
            await _db.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("api/user/transfer")]
        public async Task<IActionResult> TransferMoney(int fromId, int toId, decimal money, [FromServices] ICurrencyConverter currencyConverter)
        {
            Account accFrom;
            var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "User")
            {
                var userId = int.Parse(User.Identity.Name);
                accFrom = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == fromId);
            }
            else 
                accFrom = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == fromId);
            
            if (accFrom == null) return BadRequest();
            if (accFrom.Money < money) return BadRequest("Account dont have enough money to make a transfer");
            
            var accTo = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == toId);
            if (accTo == null) return BadRequest();

            //Find global commission information
            var glRule = await _db.GlobalMoneyRules.FirstOrDefaultAsync(r => 
                r.CurrencyId == accFrom.CurrencyId 
                && r.Min<=money 
                && r.Max>=money);
            //Find custom user commission information
            var customRule = await _db.MoneyRules.FirstOrDefaultAsync(r =>
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
            var curFrom = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == accFrom.CurrencyId);
            var curTo=await _db.Currencies.FirstOrDefaultAsync(c => c.Id == accTo.CurrencyId);
            decimal moneyToReceive = currencyConverter.ConvertMoney(money, curFrom, curTo);

            var sending = new Transfer {AccountId = fromId, Money = -moneyToTake, Type = TransferType.Transfer,};
            var delivery = new Transfer {AccountId = toId, Money = moneyToReceive, Type = TransferType.Transfer};
            
            _db.Accounts.UpdateRange(accFrom,accTo);
            await _db.Transfers.AddRangeAsync(sending,delivery);
            await _db.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("api/user/test")]
        public async Task<IActionResult> Test()
        {
            var user = await _db.Users.Include(u => u.Accounts).FirstOrDefaultAsync();
            return Json(user);
        }
        [HttpPost("api/user/test")]
        public async Task<IActionResult> TestPost([FromBody]User user)
        {
            if (!ModelState.IsValid) return BadRequest();
            var u = _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtRoute("GetUser", new {id = u.Entity.Id}, u.Entity);
        }
        [HttpPut("api/user/test")]
        public IActionResult TestPut(int id, [FromBody] User user)
        {
            //TODO ...
            return Json(user);
        }
        
    }
}