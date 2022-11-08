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
        private User _user;
        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            using (var db = new DBConnection())
            {
                _user = db.getUserByLogin(httpContextAccessor.HttpContext.User.Identities.First().Name);
            }
        }
        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            if (_user.is_admin == false)
                return NotFound();
            return View();
        }

        // POST: AdminController/GetProducts
        [HttpPost]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            if (_user.is_admin == false)
                return NotFound();
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
            if (_user.is_admin == false)
                return NotFound();
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

        //Пытаемся сделать лучше
        [HttpPost]
        public async Task<IActionResult> UpdateProductTheBest([FromBody] Product[] modifiedProducts)
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                foreach (var item in modifiedProducts)
                {
                    var updatingProduct = await db.Products.FirstOrDefaultAsync(x => x.id == item.id);
                    if (updatingProduct == null)
                    {
                        updatingProduct = new Product();
                        updatingProduct.id = item.id;
                        updatingProduct.name = item.name;
                        updatingProduct.price = item.price;
                        updatingProduct.extra = item.extra;
                        updatingProduct.availability_tomorrow = item.availability_tomorrow;
                        updatingProduct.category_id = item.category_id;
                        db.Products.Add(updatingProduct);
                    }
                    else
                    {
                        updatingProduct.id = item.id;
                        updatingProduct.name = item.name;
                        updatingProduct.price = item.price;
                        updatingProduct.extra = item.extra;
                        updatingProduct.availability_tomorrow = item.availability_tomorrow;
                        updatingProduct.category_id = item.category_id;
                    }
                }
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
        [HttpPost]
        public async Task<IActionResult> GetConfig()
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                return new OkObjectResult(db.GetConfig());
            }
        }
        [HttpPost]
        public async Task<IActionResult> SetConfig([FromBody]Config newConfig)
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                var currentConfig = db.GetConfig();
                currentConfig.last_time_to_do_order = newConfig.last_time_to_do_order;
                currentConfig.next_order_day = newConfig.next_order_day;
                await db.SaveChangesAsync();
            }
            return Ok();
        }
    }

}
