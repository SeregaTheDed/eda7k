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

        // POST: AdminController/Massiv
        [HttpPost]
        public Product[] Massiv()
        {
            Product[] products = new Product[]
            {
                new Product{id = 1, name = "Бризоль"},
                new Product{id = 2, name = "Соус"},
                new Product{id = 3, name = "Котлета"},
                new Product{id = 4, name = "Макароны"},
                new Product{id = 5, name = "Греча"},
            };
            return products;
        }
        [HttpPost]
        public Product[] Massiv2([FromBody]Product[] products)
        {
            return products;
        }
    }
    [Serializable]
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; } 
    }
}
