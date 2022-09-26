using Microsoft.AspNetCore.Mvc;

namespace eda7k.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        public IActionResult Authentication(string login)
        {
            if (login == "admin")
                return new RedirectResult("home");
            return new NotFoundResult();
            
        }
    }
}
