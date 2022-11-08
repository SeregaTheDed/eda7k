using eda7k.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace eda7k.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private User _user;
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            using (var db = new DBConnection())
            {
                _user = db.getUserByLogin(httpContextAccessor.HttpContext.User.Identities.First().Name);
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductsForOrder()
        {
            using (DBConnection db = new())
            {
                return new OkObjectResult(db.Products.Where(x => x.availability_tomorrow).ToArray());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}