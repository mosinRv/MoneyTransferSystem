using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Models;

namespace MoneyTransferSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountAdminController : Controller
    {
        private readonly MyDbContext _context;

        public AccountAdminController(MyDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("api/admin/account")]
        public async Task<ActionResult<IEnumerable<AccountDetailDto>>> GetAccounts(int count=10)
        {
            List<AccountDetailDto> accounts =  await _context.Accounts.Take(count)
                .Include(a=>a.User)
                .Include(a=>a.Currency)
                .Select(AsAccountDetailDto)
                .ToListAsync();
            return accounts;
        }
        
        [HttpGet("api/admin/account/{id}", Name = "GetAccount")]
        public async Task<ActionResult<AccountDetailDto>> GetAccountById(int id)
        {
            AccountDetailDto acc = await _context.Accounts.Where(a => a.Id == id)
                .Select(AsAccountDetailDto)
                .FirstOrDefaultAsync();
            if (acc == null)
                return NotFound();
            return acc;
        }
        
        [HttpPost("api/admin/account")]
        public async Task<IActionResult> CreateAccount([FromBody]Account acc)
        {
            var a=_context.Accounts.Add(acc);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetAccount", new {id = a.Entity.Id}, a.Entity);
        }
        
        [HttpPost("api/admin/account/transfer-confirmation")]
        public async Task<IActionResult> ConfirmTransfer(int id, bool isConfirmed)
        {
            Transfer transfer = await _context.Transfers.Where(t => t.Id == id && t.Status==TransferStatus.Pending)
                .Include(t => t.Account).FirstOrDefaultAsync();
            if (transfer == null) return NotFound();
            
            switch (transfer.Type)
            {
                case TransferType.Deposit when isConfirmed:
                    transfer.Account.Money += transfer.Money; 
                    break;
                case TransferType.Withdrawal when isConfirmed:
                    transfer.Account.Money -= transfer.Money; 
                    break;
                case TransferType.TransferTo when isConfirmed:
                    transfer.Account.Money += transfer.Money; 
                    break;
                case TransferType.TransferFrom when !isConfirmed:
                    // return frozen money
                    transfer.Account.Money += transfer.Money;
                    break;
            }

            transfer.Status = isConfirmed ? TransferStatus.Approved : TransferStatus.Declined;
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("api/admin/account/withdrawal")]
        public async Task<IActionResult> TakeMoney(int accId, decimal money)
        {
            Account acc = await _context.Accounts.Where(a => a.Id == accId)
                .Include(a => a.Currency).FirstOrDefaultAsync();
            
            if (acc == null) return NotFound();
            if (money <= 0) return BadRequest("value of money must be greater than zero");
            
            acc.Money -= money;
            
            var transfer= new Transfer 
            {
                AccountId = accId, 
                Money = money,
                Type = TransferType.Withdrawal,
                Status = TransferStatus.Approved
            };
            await _context.Transfers.AddAsync(transfer);
            
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("api/admin/account/deposit")]
        public async Task<IActionResult> DepositMoney(int accId, decimal money)
        {
            Account acc = await _context.Accounts.Where(a => a.Id == accId)
                .Include(a => a.Currency).FirstOrDefaultAsync();

            if (acc == null) return NotFound();
            if (money <= 0) return BadRequest("value of money must be greater than zero");
            
            acc.Money += money;
            
            var transfer= new Transfer 
            {
                AccountId = accId, 
                Money = money,
                Type = TransferType.Deposit,
                Status = TransferStatus.Approved
            };
            await _context.Transfers.AddAsync(transfer);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        private static readonly Expression<Func<Account, AccountDetailDto>> AsAccountDetailDto =
            acc => new AccountDetailDto
            {
                Id = acc.Id,
                CurrencyId = acc.CurrencyId,
                Currency = acc.Currency.CharCode,
                Money = acc.Money,
                UserId = acc.UserId,
                User = acc.User.Login
            };
    }
}