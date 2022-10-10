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
        /*
 $.ajax({
            method: "post",
            url: "/admin/DeleteProductById",
            data: {"id":10},
            success: function (data) {
                console.log(data);
                },
            error: function (er) {
                }
            });
         */

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
        /*
arr = [
    {
        "price": 22,
        "category_id": 2,
        "extra": 2,
        "name": "Картофельное пюре с маслом",
        "availability_tomorrow": false
    }
]

arr = JSON.stringify(arr);

 $.ajax({
            dataType: 'json',
            contentType: "application/json",
            type: "POST",
            url: "/admin/AddNewProducts",
            data: arr,
            success: function (data) {
                console.log(data);
                },
            error: function (er) {
                }
            });
         */
    }

}
