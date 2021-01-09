using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Controllers
{
    [Authorize]
    public class CurrencyController : Controller
    {
        private MyDbContext _context;

        public CurrencyController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/currency")]
        public async Task<IActionResult> GetCurrency()
        {
            return Json(await _context.Currencies.Select(x => x).ToListAsync());
        }
        [HttpGet("api/currency/{id}")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            return Json(await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id));
        }
        
        
        [HttpPost("api/currency")]
        public async Task<IActionResult> AddNewCurrency([FromBody] Currency currency)
        {
            var added=_context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetCurrency), new {id = added.Entity.Id}, added.Entity);
        }
        
        [HttpPut("api/currency")]
        public async Task<IActionResult> ChangeCurrency (int id,[FromBody] Currency currency)
        {
            Currency oldCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            if (oldCurrency == null || currency.Name==null || currency.CharCode==null) return BadRequest();
            oldCurrency.Name = currency.Name;
            oldCurrency.CharCode = currency.CharCode;
            _context.Currencies.Update(oldCurrency);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("api/currency")]
        public async Task<IActionResult> Delete(int id)
        {
            var curr = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            _context.Currencies.Remove(curr);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}