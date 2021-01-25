using System.Collections.Generic;
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
    [Authorize(Roles = "User")]
    public class CommissionController : Controller
    {
        private const int NumberOfCommissions = 10;
        private readonly MyDbContext _context;

        public CommissionController(MyDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("api/commission/global"),]
        public async Task<ActionResult<IEnumerable<CommissionDto>>> GetListGlobal()
        {
            List<CommissionDto> rules = await _context.GlobalMoneyRules.Take(NumberOfCommissions)
                .Include(comm => comm.Currency)
                .Select(comm => new CommissionDto
                {
                    Currency = comm.Currency.CharCode,
                    Max = comm.Max,
                    Min = comm.Min,
                    Commission = comm.isCommissionFixed ? comm.Commission.ToString() : $"{comm.Commission * 100}%"
                }).ToListAsync();
            return rules;
        }
        
        
        [HttpGet("api/commission/custom")]
        public async Task<ActionResult<IEnumerable<CommissionDto>>> GetListCustom()
        {
            var userId = int.Parse(User.Identity.Name);
            var rules = await _context.MoneyRules.Where(r => r.UserId == userId).Take(NumberOfCommissions)
                .Include(r=> r.Currency)
                .Select(r => new CommissionDto()
                {
                    Currency = r.Currency.CharCode,
                    Max = r.Max,
                    Min = r.Min,
                    Commission = r.isCommissionFixed ? r.Commission.ToString() : $"{r.Commission * 100}%"
                }).ToListAsync();
            return rules;
        }
        
    }
}