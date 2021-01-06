using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Controllers
{
    [Route("api/currency")]
    public class CurrencyController : Controller
    {
        private MyDbContext _db;

        public CurrencyController(MyDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrency()
        {
            return Json(await _db.Currencies.Select(x => x).ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            return Json(await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id));
        }
        
        
        [HttpPost]
        public async Task<IActionResult> AddNewCurrency([FromBody] Currency currency)
        {
            var added=_db.Currencies.Add(currency);
            await _db.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetCurrency), new {id = added.Entity.Id}, added.Entity);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> TChangeCurrency (int id, Currency currency)
        {
            Currency oldCurrency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            if (oldCurrency == null || currency.Name==null || currency.CharCode==null) return BadRequest();
            oldCurrency.Name = currency.Name;
            oldCurrency.CharCode = currency.CharCode;
            _db.Currencies.Update(oldCurrency);
            await _db.SaveChangesAsync();
            return Ok();
        }
        
    }
}