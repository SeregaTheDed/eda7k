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
        /*
        $.ajax({
            method: "POST",
            url: "/account/login",
            data: { login : "i.ivanov"},
            success: function (data) {
                $("#textbox").text($("#textbox").text() + " " +data);
                },
            error: function (er) {
                console.log(er);
                console.log(123);
                }
            });
         */


        [HttpPost]
        public async Task<IActionResult> Register(string login)
        {
            using (var db = new DBConnection())
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
        /*
        $.ajax({
            method: "POST",
            url: "/account/logout",
            success: function (data) {
                $("#textbox").text($("#textbox").text() + " " +data);
                },
            error: function (er) {
                console.log(er);
                console.log(123);
                }
            });
         */
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
