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
    public class OrderProduct
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }
    public class ProductWithCount
    {
        public Product Product { get; set; }
        public int Count { get; set; }
    }
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
            return View(_user);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductsForOrder()
        {
            using (DBConnection db = new())
            {
                if (DateTime.Now > (await db.Config.FirstAsync()).last_time_to_do_order)
                    return new OkObjectResult(new Product[0]);

                return new OkObjectResult(db.Products.Where(x => x.availability_tomorrow).ToArray());
            }
        }

        [HttpPost]
        public async Task<IActionResult> DoOrder([FromBody] OrderProduct[] productIdsAndCounts)
        {
            using (DBConnection db = new())
            {
                if (productIdsAndCounts.Length == 0)
                    return BadRequest();
                var config = await db.GetConfigAsync();
                Order currentOrder = new Order
                {
                    customer_name = _user.last_name,
                    date = config.next_order_day,
                    status_id = 1,
                    user_id= _user.id,
                };
                db.Orders.Add(currentOrder);
                await db.SaveChangesAsync();
                int idOfCurrentOrder = currentOrder.id.Value;
                foreach (var orderProduct in productIdsAndCounts.Where(x => x.Count > 0))
                {
                    db.Rel_orders_products.Add(new Rel_orders_product
                    {
                        order_id = idOfCurrentOrder,
                        product_id = orderProduct.ProductId,
                        count = orderProduct.Count,
                });
                }
                await db.SaveChangesAsync();
                return new OkResult();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllOrders()
        {
            using (DBConnection db = new())
            {
                // TODO: optimize queries
                var allOrderIds = await db.Orders
                    .Where(x => x.user_id == _user.id)
                    .Select(x => x.id)
                    .ToArrayAsync();
                List<List<ProductWithCount >> orders = new List<List<ProductWithCount>>();
                var Products = await db.Products.ToArrayAsync();
                var Rel_orders_products = await db.Rel_orders_products.ToArrayAsync();
                foreach (var OrderId in allOrderIds)
                {
                    List<ProductWithCount> currentOrder = new List<ProductWithCount>();
                    foreach (var item in Rel_orders_products
                        .Where(x => x.order_id == OrderId).ToArray())
                    {
                        var currentProduct = Products.FirstOrDefault(x => x.id == item.product_id);
                        if (currentProduct == null)
                            currentProduct = Product.GetEmptyProduct();
                        currentOrder.Add(new ProductWithCount
                        {
                            Product = currentProduct,
                            Count = item.count
                        });
                    }
                    if (currentOrder.Count > 0)
                        orders.Add(currentOrder);
                }
                return new OkObjectResult(orders);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUserName()
        {
            using (DBConnection db = new())
            {
                return new OkObjectResult(_user.last_name);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetUserName([FromBody] string name)
        {
            if (name.Length <= 3)
                return BadRequest();
            using (DBConnection db = new())
            {
                db.Users.First(x => _user.id == x.id).last_name = name;
                await db.SaveChangesAsync();
                return Ok();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}