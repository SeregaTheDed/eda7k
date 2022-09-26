using Microsoft.AspNetCore.Mvc;

namespace eda7k.Controllers
{
    public class AccountController : Controller
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
        /*
$.ajax({
    type: "POST",
    contentType: "application/json",
    url: "/account/login",
    data: JSON.stringify("admin"),
    }
);
         */
        [HttpPost]
        public IActionResult Logout()
        {
            return Redirect("~/account/auth");
        }
        /*
 $.ajax({
     type: "POST",
     contentType: "application/json",
     url: "/account/logout",
     }
 );
         */
    }
}
