using eda7k.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eda7k.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            using (var db = new ApplicationContext())
            {
                var temp = DateTime.Now;
                //Console.WriteLine(getUsers(db).Length);
                Console.WriteLine(db.getUsers().Length);
                Console.WriteLine(DateTime.Now - temp);
            }
            using (var db = new ApplicationContext())
            {
                var temp = DateTime.Now;
                //Console.WriteLine(getUsers(db).Length);
                Console.WriteLine(db.getUsers().Length);
                Console.WriteLine(DateTime.Now-temp);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}