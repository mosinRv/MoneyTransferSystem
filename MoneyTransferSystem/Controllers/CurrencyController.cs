using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly MyDbContext _context;
        private const string AdminRole = "Admin";
        private const string UserAdminRole = "User, Admin";

        public CurrencyController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = UserAdminRole)]
        [HttpGet("api/currency")]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrency()
        {
            return await _context.Currencies.ToListAsync();
        }
        
        [Authorize(Roles = UserAdminRole)]
        [HttpGet("api/currency/{id}", Name = "GetCurrency")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            if (currency == null) return BadRequest();
            return currency;
        }

        [Authorize(Roles = AdminRole)]
        [HttpPost("api/currency")]
        public async Task<IActionResult> AddNewCurrency([FromBody] Currency currency)
        {
            var added=_context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetCurrency", new {id = added.Entity.Id}, added.Entity);
        }
        
        [Authorize(Roles = AdminRole)]
        [HttpPut("api/currency")]
        public async Task<IActionResult> ChangeCurrency (int id,[FromBody] Currency currency)
        {
            Currency oldCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            if (oldCurrency == null)return BadRequest();
            if (currency.Name==null || currency.CharCode==null) return BadRequest();
            oldCurrency.Name = currency.Name;
            oldCurrency.CharCode = currency.CharCode;
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = AdminRole)]
        [HttpDelete("api/currency")]
        public async Task<IActionResult> Delete(int id)
        {
            var curr = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            if (curr == null) return BadRequest();
            _context.Currencies.Remove(curr);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}