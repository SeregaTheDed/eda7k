using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public Product[] Massiv(IFormCollection collection)
        {
            Product[] products = new Product[]
            {
                new Product{Id = 1, Name = "123"},
                new Product{Id = 2, Name = "123"},
                new Product{Id = 3, Name = "123"},
            };
            return products;
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
