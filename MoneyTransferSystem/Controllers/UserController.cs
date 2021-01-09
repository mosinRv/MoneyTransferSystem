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
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly MyDbContext _context;

        public UserController(MyDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("api/user")]
        public async Task<JsonResult> GetUsers(int count=10, int page=1)
        {
            var users = await _context.Users.Skip(count*(page-1)).Take(count)
                .Include(u => u.Accounts)
                .ThenInclude(a => a.Currency)
                .Select(u => new
                {
                    Id = u.Id,
                    Login = u.Login,
                    Role = u.isAdmin ? "Admin" : "User",
                    Accounts = u.Accounts.Select(a => new
                    {
                        Id = a.Id,
                        CurrencyId = a.CurrencyId,
                        Currency = a.Currency.CharCode,
                        Money = a.Money
                    })
                }).ToListAsync();
            return Json(users);
        }
        
        [HttpGet("api/user/{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user== null)
                return NotFound();
            await _context.Entry(user).Collection(u => u.Accounts).LoadAsync();
            var result = new
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.isAdmin ? "Admin" : "User",
                Accounts = user.Accounts.Select(a => new
                {
                    Id = a.Id,
                    CurrencyId = a.CurrencyId,
                    Currency = a.Currency.CharCode,
                    Money = a.Money
                })
            };
            return Json(result);
        }
        [HttpPost("api/user")]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {
            try
            {
                var u = _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetUser", new {id = u.Entity.Id}, u.Entity);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}