using Microsoft.AspNetCore.Mvc;

namespace eda7k.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        public IActionResult Authentication(string login, string password)
        {
            if (login == "admin" && password == "admin")
                return new OkObjectResult("COMPLETED");
            return new NotFoundResult();
            
        }
    }
}
