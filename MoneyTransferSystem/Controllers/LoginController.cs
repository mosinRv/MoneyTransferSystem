using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyTransferSystem.Database;

namespace MoneyTransferSystem.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private MyDbContext _db;
        public LoginController(MyDbContext db)
        {
            _db = db;
        }
        public IActionResult UserLogin()
        {
            return View();
        }
        
        [HttpPost("sign-in")]
        public async Task<IActionResult> LogIn(string login, string pass)
        {
            var user = _db.Users.FirstOrDefault(u=>u.Login==login);
            if (user == null || user.Pass != pass)
                return BadRequest();
            
            var role = user.isAdmin ? "Admin" : "User";
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            await HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType)));
            return Ok();
        }
        [HttpPost("sign-out")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet("test"), Authorize()]
        public IActionResult TestAuthorization()
        {
            var userId = User.Identity.Name;
            var role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            return Content($"{userId} ({role})");
        }
        
    }
}