using eda7k.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace eda7k.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: AdminController/auth
        public async Task<IActionResult> Auth()
        {
            return View();
        }

        // POST: AdminController/GetProducts
        [HttpPost]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            using (DBConnection db = new())
            {
                var deletingProduct = await db.Products.FirstOrDefaultAsync(x => x.id == id);
                db.Products.Remove(deletingProduct);
                await db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody]Product modifiedProduct)
        {
            using (DBConnection db = new())
            {
                var updatingProduct = await db.Products.FirstOrDefaultAsync(x => x.id == modifiedProduct.id);
                if (updatingProduct == null)
                    return BadRequest();
                updatingProduct.id = modifiedProduct.id;
                updatingProduct.name = modifiedProduct.name;
                updatingProduct.price = modifiedProduct.price;
                updatingProduct.extra = modifiedProduct.extra;
                updatingProduct.availability_tomorrow = modifiedProduct.availability_tomorrow;
                updatingProduct.category_id = modifiedProduct.category_id;
                await db.SaveChangesAsync();
            }
            return Ok();
        }

        // POST: AdminController/GetProducts
        [HttpPost]
        public async Task<IActionResult> GetCategories()
        {
            using (DBConnection db = new())
            {
                return new OkObjectResult(db.Categories.ToArray());
            }
        }

        // POST: AdminController/GetProducts
        [HttpPost]
        public async Task<IActionResult> GetProducts()
        {
            using (DBConnection db = new())
            {
                return new OkObjectResult(db.Products.ToArray());
            }
        }

        /// <summary>
        /// Принимает список новых продуктов и возвращает список всех продуктов(включая новые)
        /// </summary>
        /// <param name="products"></param>
        /// <returns>Cписок всех продуктов(включая новые)</returns>
        [HttpPost]
        public async Task<IActionResult> AddNewProducts([FromBody] Product[] products)
        {
            using (DBConnection db = new())
            {
                await db.Products.AddRangeAsync(products);
                await db.SaveChangesAsync();
                return new OkObjectResult(db.Products.ToArray());
            }
            
        }
    }

}
