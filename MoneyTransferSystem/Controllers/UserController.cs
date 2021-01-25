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
using MoneyTransferSystem.Models;
using MoneyTransferSystem.Services;

namespace MoneyTransferSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _context;
        private const string AdminRole = "Admin";
        private const string UserAdminRole = "User, Admin";

        public UserController(MyDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("api/user"), Authorize(Roles = UserAdminRole)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int count=10, int page=1)
        {
            var users = await _context.Users.Skip(count*(page-1)).Take(count)
                .Include(u => u.Accounts)
                .ThenInclude(a => a.Currency)
                .Select(u => new UserDto
                {
                    User = u.Login,
                    Role = u.isAdmin ? "Admin" : "User",
                    Accounts = u.Accounts.Select(a => new AccountDto()
                    {
                        Id = a.Id,
                        CurrencyId = a.CurrencyId,
                        Currency = a.Currency.CharCode,
                        Money = a.Money
                    })
                }).ToListAsync();
            return users;
        }
        
        [Authorize(Roles = UserAdminRole)]
        [HttpGet("api/user/{id}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.Where(u => u.Id == id)
                .Include(u => u.Accounts)
                .Select(u=>new UserDto
                {
                    User = u.Login,
                    Role = u.isAdmin ? "Admin" : "User",
                    Accounts = u.Accounts.Select(a => new AccountDto()
                    {
                        Id = a.Id,
                        CurrencyId = a.CurrencyId,
                        Currency = a.Currency.CharCode,
                        Money = a.Money
                    })
                }).FirstOrDefaultAsync();
            if (user== null)
                return NotFound();
            
            return user;
        }
        
        [HttpPost("api/user"), Authorize(Roles = AdminRole)]
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