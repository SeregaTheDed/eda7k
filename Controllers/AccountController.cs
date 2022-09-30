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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationContext())
                {
                    User user = db.getUserByLogin(login);
                    if (user != null)
                    {
                        await Authenticate(login); // аутентификация

                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string login)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationContext())
                {


                    User user = db.getUserByLogin(login);
                    if (user == null)
                    {
                        // добавляем пользователя в бд
                        db.Users.Add(new User { login = login });
                        await db.SaveChangesAsync();

                        await Authenticate(login); // аутентификация

                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
            return View("auth");
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






    /*public class AccountController : Controller
    {
        public IActionResult Auth()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromBody]string login)
        {
            if (login == "admin")
                return Redirect("~/home");
            return new NotFoundObjectResult("123");
            
        }
        *//*
$.ajax({
    type: "POST",
    contentType: "application/json",
    url: "/account/login",
    data: JSON.stringify("admin"),
    }
);
         *//*
        [HttpPost]
        public IActionResult Logout()
        {
            return Redirect("~/account/auth");
        }
        *//*
 $.ajax({
     type: "POST",
     contentType: "application/json",
     url: "/account/logout",
     }
 );
         *//*
    }*/
}
