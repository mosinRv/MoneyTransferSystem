using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTransferSystem.Database;
using MoneyTransferSystem.Database.DbModels;

namespace MoneyTransferSystem.Controllers
{
    [Route("api/commission"), Authorize(Roles = "User, Admin")]
    public class CommissionController : Controller
    {
        private const int jsonReturnNumb = 10;
        private MyDbContext _db;

        public CommissionController(MyDbContext db)
        {
            _db = db;
        }

        #region global rules
        [HttpGet("global"),]
        public async Task<IActionResult> GetListGlobal()
        {
            var rules = await _db.GlobalMoneyRules.Take(jsonReturnNumb).Select(r => new
            {
                Currency = r.CurrencyId,
                Max = r.Max,
                Min = r.Min,
                Comission = r.isCommissionFixed ? r.Commission.ToString() : $"{r.Commission * 100}%"
            }).ToListAsync();
            return Json(rules);
        }
        
        [HttpGet("global/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGlobal(int id)
        {
            return Json(await _db.GlobalMoneyRules.FirstOrDefaultAsync(r => r.Id == id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addGlobal")]
        public async Task<IActionResult> CreateCommission([FromBody]GlobalCommission rule)
        {
            var added=_db.GlobalMoneyRules.Add(rule);
            await _db.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetGlobal), new {id = added.Entity.Id}, added.Entity);
        }
        #endregion
        
        //-------------------------
        
        #region custom rules
        [HttpGet("custom")]
        public async Task<IActionResult> GetListCustom()
        {
            var userId = int.Parse(User.Identity.Name);
            var rules = await _db.MoneyRules.Where(r => r.UserId == userId).Take(jsonReturnNumb).Select(r => new
            {
                Currency = r.CurrencyId,
                Max = r.Max,
                Min = r.Min,
                Comission = r.isCommissionFixed ? r.Commission.ToString() : $"{r.Commission * 100}%"
            }).ToListAsync();
            return Json(rules);
        }
        
        [HttpGet("custom/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustom(int id)
        {
            return Json(await _db.MoneyRules.FirstOrDefaultAsync(r => r.Id == id));
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("addCustom")]
        public async Task<IActionResult> CreateUserCommission([FromBody]PersonalCommission rule)
        {
            var added=_db.MoneyRules.Add(rule);
            await _db.SaveChangesAsync();
            return CreatedAtRoute(nameof(GetCustom), new {id = added.Entity.Id}, added.Entity);
        }
        
        
        #endregion
    }
}