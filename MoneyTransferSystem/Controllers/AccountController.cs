using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Models;
using MoneyTransferSystem.Services;

namespace MoneyTransferSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;

        public AccountController(MyDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/account")]
        public async Task<ActionResult<UserDto>> GetCurrentUserInfo()
        {
            var userId = int.Parse(User.Identity.Name);
            UserDto user = await _context.Users.Where(u => u.Id == userId).Include(u => u.Accounts)
                .ThenInclude(a => a.Currency).Select(u => new UserDto
                {
                    User = u.Login,
                    Role = u.isAdmin ? "Admin" : "User",
                    Accounts = u.Accounts.Select(a => new AccountDto
                    {
                        Id = a.Id,
                        CurrencyId = a.CurrencyId,
                        Currency = a.Currency.CharCode,
                        Money = a.Money
                    })
                }).FirstOrDefaultAsync();
            
            return Ok(user);
        }

        [HttpPost("api/account/withdraw")]
        public async Task<IActionResult> WithdrawMoney(int accId, decimal money)
        {
            var userId = int.Parse(User.Identity.Name);
            Account acc = await _context.Accounts.Where(a => a.Id == accId && a.UserId == userId)
                .Include(a => a.Currency).FirstOrDefaultAsync();

            if (acc == null) return NotFound();
            if (money <= 0) return BadRequest("value of money must be greater than zero");
            
            decimal moneyToTake = await ChargeCommissionForTransfer(money, acc.CurrencyId, userId);
            if (acc.Money < moneyToTake)
                return BadRequest(
                    "There are not enough funds on the account for the withdrawal, taking into consideration the fee");

            
            TransferStatus status = moneyToTake > acc.Currency.MaxTransferSize ? TransferStatus.Pending : TransferStatus.Approved;
            if (status == TransferStatus.Approved)
                acc.Money -= moneyToTake;
            
            
            var transfer= new Transfer 
            {
                AccountId = accId, 
                Money = moneyToTake,
                Type = TransferType.Withdrawal,
                Status = status
            };
            await _context.Transfers.AddAsync(transfer);
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("api/account/deposit")]
        public async Task<IActionResult> DepositMoney(int accId, decimal money)
        {
            var userId = int.Parse(User.Identity.Name);
            Account acc = await _context.Accounts.Where(a => a.Id == accId && a.UserId == userId)
                .Include(a => a.Currency).FirstOrDefaultAsync();

            if (acc == null) return NotFound();
            if (money <= 0) return BadRequest("value of money must be greater than zero");
            
            //take commission from money, and the rest will be deposited into account
            decimal commission = await CalculateCommissionForDeposit(money, acc.CurrencyId, userId);
            decimal depositMoney = money - commission;
            if (depositMoney <= 0) return BadRequest("value of money must be greater than zero after taking the commission");
            
            TransferStatus status = depositMoney > acc.Currency.MaxTransferSize ? TransferStatus.Pending : TransferStatus.Approved;
            if (status == TransferStatus.Approved)
                acc.Money += depositMoney;
            
            var transfer= new Transfer 
            {
                AccountId = accId, 
                Money = depositMoney,
                Type = TransferType.Deposit,
                Status = status
            };
            await _context.Transfers.AddAsync(transfer);
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
            
            decimal moneyToTake = await ChargeCommissionForTransfer(money, accFrom.CurrencyId, userId);
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
            TransferStatus status = (needConfirmToReceive && needConfirmToTake) 
                ? TransferStatus.Pending : TransferStatus.Approved;

            // the sender's money will be frozen and will awaits confirmation
            accFrom.Money -= moneyToTake;
            if (status == TransferStatus.Approved)
                accTo.Money += moneyToReceive;
            
            var sending = new Transfer {AccountId = fromId, Money = moneyToTake, Type = TransferType.TransferFrom, Status = status};
            var delivery = new Transfer {AccountId = toId, Money = moneyToReceive, Type = TransferType.TransferTo, Status = status};
            
            _context.Accounts.UpdateRange(accFrom,accTo);
            await _context.Transfers.AddRangeAsync(sending,delivery);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<decimal> ChargeCommissionForTransfer(decimal moneyToTake, int currencyId, int userId)
        {
            decimal commission = 0;
            var customRule = await _context.MoneyRules.FirstOrDefaultAsync(c =>
                c.CurrencyId == currencyId && c.UserId==userId );
            if (customRule != null)
            {
                commission = customRule.isCommissionFixed
                    ? customRule.Commission
                    : moneyToTake / (1.0m - customRule.Commission)*customRule.Commission;

                if (commission > customRule.Max) commission = customRule.Max;
                else if (commission < customRule.Min) commission = customRule.Min;
            }
            else
            {
                var glRule = await _context.GlobalMoneyRules.FirstOrDefaultAsync(c =>
                    c.CurrencyId == currencyId);
                if (glRule != null)
                {
                    commission = glRule.isCommissionFixed
                        ? glRule.Commission
                        : moneyToTake / (1.0m - glRule.Commission)*glRule.Commission * glRule.Commission;
                    
                    if (commission > glRule.Max) commission = glRule.Max;
                    else if (commission < glRule.Min) commission = glRule.Min;
                }
            }

            moneyToTake += commission;
            return moneyToTake;
        }

        private async Task<decimal> CalculateCommissionForDeposit(decimal money, int currencyId, int userId)
        {
            decimal commission = 0;
            var customRule = await _context.MoneyRules.FirstOrDefaultAsync(c =>
                c.CurrencyId == currencyId && c.UserId==userId);
            if (customRule != null)
            {
                commission = customRule.isCommissionFixed
                    ? customRule.Commission
                    : money * customRule.Commission;

                if (commission > customRule.Max) commission = customRule.Max;
                else if (commission < customRule.Min) commission = customRule.Min;
            }
            else
            {
                var glRule = await _context.GlobalMoneyRules.FirstOrDefaultAsync(c =>
                    c.CurrencyId == currencyId);
                if (glRule != null)
                {
                    commission = glRule.isCommissionFixed
                        ? glRule.Commission
                        : money * glRule.Commission;
                    
                    if (commission > glRule.Max) commission = glRule.Max;
                    else if (commission < glRule.Min) commission = glRule.Min;
                }
            }

            return commission;
        }

    }
}