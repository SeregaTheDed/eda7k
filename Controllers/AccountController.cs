using eda7k.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace eda7k.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login)
        {
            using (var db = new DBConnection())
            {
                User user = db.getUserByLogin(login);
                if (user != null)
                {
                    await Authenticate(login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }

                return new NotFoundResult();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string login)
        {
            using (var db = new DBConnection())
            {
                User user = db.getUserByLogin(login);
                if (user == null)
                {
                    db.Users.Add(new User { login = login });
                    await db.SaveChangesAsync();

                    await Authenticate(login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            } 
            return View("Login");
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        
    }

}
