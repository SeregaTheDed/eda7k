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
            //(Product, int)[]
            using (DBConnection db = new())
            {
                // TODO: optimize queries
                var allOrderIds = db.Orders
                    .Where(x => x.user_id == _user.id)
                    .Select(x => x.id);
                List<List<(Product, int)>> orders = new List<List<(Product, int)>>();

                foreach (var OrderId in allOrderIds)
                {
                    List<(Product, int)> currentOrder = new List<(Product, int)>();
                    foreach (var item in db.Rel_orders_products
                        .Where(x => x.order_id == OrderId))
                    {
                        var currentProduct = await db.Products.FirstOrDefaultAsync(x => x.id == item.product_id);
                        if (currentProduct == null)
                            currentProduct = Product.GetEmptyProduct();
                        currentOrder.Add((currentProduct, item.count));
                    }
                    orders.Add(currentOrder);
                }
                return new OkObjectResult(orders);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}