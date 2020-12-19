using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyTransferSystem.Database;

namespace MoneyTransferSystem.Controllers
{
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

        public async Task Login(string login, string pass)
        {
            var user = _db.Users.FirstOrDefault(u=>u.Login==login);
            if (user != null && user.Pass==pass)
            {
                var role = user.isAdmin ? "Admin" : "User";
                var claims = new List<Claim>
                {
                    new Claim("login", user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                await HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", "login", ClaimsIdentity.DefaultRoleClaimType)));
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else Response.StatusCode = 404;
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("UserLogin", "Login");
        }

        public string GetUserLogin()
        {
            ClaimsPrincipal principal = HttpContext.User;
            Claim login = principal.FindFirst("login");
            string s = login?.Value ?? "Not Authorized";
            return s;
        }
    }
}