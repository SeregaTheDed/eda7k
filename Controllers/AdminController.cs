using eda7k.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace eda7k.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            if (false)
                return RedirectToAction("Auth");
            return View();
        }
        // GET: AdminController/auth
        public ActionResult Auth()
        {
            if (false)
                return RedirectToAction("Index");
            return View();
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Massiv
        public ActionResult Massiv()
        {
            return View();
        }

        // POST: AdminController/Massiv
        [HttpPost]
        public User[] Massiv(int a)
        {
            using (var db = new ApplicationContext())
            {
                return db.users.ToArray();
            }
           /* Product[] products = new Product[]
            {
                new Product{Id = 1, Name = "Бризоль"},
                new Product{Id = 2, Name = "Соус"},
                new Product{Id = 3, Name = "Котлета"},
                new Product{Id = 4, Name = "Макароны"},
                new Product{Id = 5, Name = "Греча"},
            };
            return products;*/
        }
        [HttpPost]
        public Product[] Massiv2(string abc)
        {
            Product[] mass = JsonSerializer.Deserialize<Product[]>(abc);
            if (mass.Count() != 0)
                return new Product[]
            {
                new Product { Id = 1, Name = "Бризоль" },
                new Product { Id = 2, Name = "Соус" },
                new Product { Id = 3, Name = "Котлета" },
                new Product { Id = 4, Name = "Макароны" },
                new Product { Id = 5, Name = "Греча" },
            };
            return new Product[0];
        }
    }
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
