using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class CommissionAdminController : Controller
    {
        private readonly MyDbContext _context;
        private const int NumberOfCommissions = 10;

        public CommissionAdminController(MyDbContext context)
        {
            _context = context;
        }
        
        // Global commissions:
        [HttpGet("api/admin/commission/global"),]
        public async Task<ActionResult<IEnumerable<CommissionDetailDto>>> GetListGlobal()
        {
            List<CommissionDetailDto> rules = await _context.GlobalMoneyRules.Take(NumberOfCommissions)
                .Include(comm => comm.Currency)
                .Select(comm => new CommissionDetailDto
                {
                    Id = comm.Id,
                    CurrencyId = comm.CurrencyId,
                    Currency = comm.Currency.CharCode,
                    Max = comm.Max,
                    Min = comm.Min,
                    Commission = comm.isCommissionFixed ? comm.Commission.ToString() : $"{comm.Commission * 100}%"
                }).ToListAsync();
            return rules;
        }
        
        [HttpGet("api/admin/commission/global/{id}")]
        public async Task<ActionResult<CommissionDetailDto>> GetGlobal(int id)
        {
            CommissionDetailDto rule = await _context.GlobalMoneyRules.Where(comm => comm.Id == id)
                .Include(comm => comm.Currency)
                .Select(comm => new CommissionDetailDto
                {
                    Id = comm.Id,
                    CurrencyId = comm.CurrencyId,
                    Currency = comm.Currency.CharCode,
                    Max = comm.Max,
                    Min = comm.Min,
                    Commission = comm.isCommissionFixed ? comm.Commission.ToString() : $"{comm.Commission * 100}%"
                }).FirstOrDefaultAsync();
            return rule;
        }
        
        [HttpPost("api/admin/commission/global")]
        public async Task<IActionResult> CreateGlobalCommission([FromBody]GlobalCommission rule)
        {
            var added=_context.GlobalMoneyRules.Add(rule);
            await _context.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetGlobal), new {id = added.Entity.Id}, added.Entity);
        }
        
        // Custom commissions:
        [HttpGet("api/admin/commission/custom")]
        public async Task<ActionResult<IEnumerable<CustomCommissionDetailDto>>> GetListCustom()
        {
            var rules = await _context.MoneyRules.Take(NumberOfCommissions)
                .Include(r=>r.Currency)
                .Include(r=>r.User)
                .Select(r => new CustomCommissionDetailDto
                {
                    Id = r.Id,
                    CurrencyId = r.CurrencyId,
                    Currency = r.Currency.CharCode,
                    UserId = r.UserId,
                    User = r.User.Login,
                    Max = r.Max,
                    Min = r.Min,
                    Commission = r.isCommissionFixed ? r.Commission.ToString() : $"{r.Commission * 100}%"
                }).ToListAsync();
            return rules;
        }
        
        [HttpGet("api/admin/commission/custom/{id}")]
        public async Task<ActionResult<CustomCommissionDetailDto>> GetCustom(int id)
        {
            CustomCommissionDetailDto rule = await _context.MoneyRules.Where(r => r.Id == id)
                .Include(r => r.Currency)
                .Include(r => r.User)
                .Select(r => new CustomCommissionDetailDto
                {
                    Id = r.Id,
                    CurrencyId = r.CurrencyId,
                    Currency = r.Currency.CharCode,
                    UserId = r.UserId,
                    User = r.User.Login,
                    Max = r.Max,
                    Min = r.Min,
                    Commission = r.isCommissionFixed ? r.Commission.ToString() : $"{r.Commission * 100}%"
                }).FirstOrDefaultAsync();
            return rule;
        }
        
        [HttpPost("api/admin/commission/custom")]
        public async Task<IActionResult> CreateCustomCommission([FromBody]PersonalCommission rule)
        {
            var added=_context.MoneyRules.Add(rule);
            await _context.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetCustom), new {id = added.Entity.Id}, added.Entity);
        }
    }
}