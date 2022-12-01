using eda7k.Models;
using eda7k.Models.MyChannel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace eda7k.Controllers
{
    public class PositionsController : Controller
    {
        private User _user;
        public PositionsController(IHttpContextAccessor httpContextAccessor)
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
        public async Task<IActionResult> AddNewPosition([FromBody] Position newPosition)
        {
            using (var db = new DBConnection())
            {
                db.Positions.Add(newPosition);
                await db.SaveChangesAsync();
                //Check here firstly if problem
                await MyChannel.WriteAsync(new KeyValuePair<Operations, int>(Operations.Add, newPosition.id.Value));
            }
            return new OkResult();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPositions()
        {
            using (var db = new DBConnection())
            {
                var Positions = await db.Positions.ToArrayAsync();
                var ProductsById = (await db.Products.ToArrayAsync()).ToDictionary(x => x.id.Value);

                var PositionViews = Positions.Select(x =>
                {
                    return GetPositionViewFromPosition(x, ProductsById);
                });

                return new OkObjectResult(Positions);
            }
        }

        private static PositionView GetPositionViewFromPosition(Position position, Dictionary<int?, Product> ProductsById)
        {
            StringBuilder productNameSB = new StringBuilder();

            Product product1 = ProductsById[position.product_id_first];
            productNameSB.Append(product1.name);

            if (position.product_id_second.HasValue)
            {
                Product product2;
                if (ProductsById.ContainsKey(position.product_id_second))
                {
                    product2 = ProductsById[position.product_id_second];
                }
                else
                {
                    product2 = Product.GetEmptyProduct();
                }
                productNameSB.Append(" + " + product2.name);
            }

            if (position.with_sauce)
            {
                productNameSB.Append(" + соус");
            }

            return new PositionView()
            {
                Id = position.id.Value,
                Name = productNameSB.ToString(),
                Price = position.price

            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class PositionView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

    }
}
