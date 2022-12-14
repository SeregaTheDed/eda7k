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
                deletingProduct.trash = true;
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
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                return new OkObjectResult(await db.Categories.ToArrayAsync());
            }
        }

        // POST: AdminController/GetProducts
        [HttpPost]
        public async Task<IActionResult> GetProducts()
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                return new OkObjectResult(await db.Products.Where(x => x.trash == false).ToArrayAsync());
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
            if (_user.is_admin == false)
                return NotFound();
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
                return new OkObjectResult(await db.GetConfigAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetConfig([FromBody]Config newConfig)
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                var currentConfig = await db.Config.FirstOrDefaultAsync();
                currentConfig.last_time_to_do_order = newConfig.last_time_to_do_order;
                currentConfig.next_order_day = newConfig.next_order_day;
                await db.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetStatuses()
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                return new OkObjectResult(await db.Statuses.ToArrayAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllOrders()
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                var allOrders = await db.Orders
                    .ToArrayAsync();
                List<AdminOrder> orders = new List<AdminOrder>();
                var Products = await db.Products.ToArrayAsync();
                var Rel_orders_products = await db.Rel_orders_products.ToArrayAsync();
                foreach (var Order in allOrders)
                {
                    int OrderId = Order.id.Value;
                    List<ProductWithCount> currentOrderProducts = new List<ProductWithCount>();
                    foreach (var item in Rel_orders_products
                        .Where(x => x.order_id == OrderId).ToArray())
                    {
                        var currentProduct = Products.FirstOrDefault(x => x.id == item.product_id);
                        if (currentProduct == null)
                            currentProduct = Product.GetEmptyProduct();
                        currentOrderProducts.Add(new ProductWithCount
                        {
                            Product = currentProduct,
                            Count = item.count
                        });
                    }
                    if (currentOrderProducts.Count > 0)
                    {
                        orders.Add(new AdminOrder()
                        {
                            Products = currentOrderProducts,
                            CustomerName = Order.customer_name,
                            OrderId = OrderId,
                            StatusId = Order.status_id,
                        });
                    }
                }
                return new OkObjectResult(orders);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatusById(int id, int newStatusId)
        {
            using (DBConnection db = new())
            {
                var needOrder = await db.Orders.FirstAsync(x => x.id == id);
                needOrder.status_id = newStatusId;
                await db.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPositions([FromBody]Position[] newPositions)
        {
            using(DBConnection db = new())
            {
                foreach (var position in newPositions)
                {
                    position.date = DateTime.Today;
                    db.Positions.Add(position);
                }
                await db.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPositions()
        {
            using (DBConnection db = new())
            {
                return new OkObjectResult(await db.Positions.ToArrayAsync());
            }
        }
    }
    class AdminOrder
    {
        public List<ProductWithCount> Products { get; set; } = new List<ProductWithCount>();
        public string CustomerName { get; set; }
        public int StatusId { get; set; }
        public int OrderId { get; set; }
    }

}
