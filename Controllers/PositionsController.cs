using eda7k.Models;
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

        public async Task<IActionResult> GetAllPositions()
        {
            using (var db = new DBConnection())
            {
                var Positions = await db.Positions.ToArrayAsync();
                var ProductsById = (await db.Products.ToArrayAsync()).ToDictionary(x => x.id);

                var PositionViews = Positions.Select(x =>
                {
                    return GetProductView(x, ProductsById);
                });

                return new OkObjectResult(Positions);
            }
        }

        private static PositionView GetProductView(Position x, Dictionary<int?, Product> ProductsById)
        {
            StringBuilder productNameSB = new StringBuilder();

            Product product1 = ProductsById[x.product_id_first];
            productNameSB.Append(product1.name);

            if (x.product_id_second.HasValue)
            {
                Product product2;
                if (ProductsById.ContainsKey(x.product_id_second))
                {
                    product2 = ProductsById[x.product_id_second];
                }
                else
                {
                    product2 = Product.GetEmptyProduct();
                }
                productNameSB.Append(" + " + product1.name);
            }

            if (x.with_sauce)
            {
                productNameSB.Append(" + соус");
            }

            return new PositionView()
            {
                Id = x.id.Value,
                Name = productNameSB.ToString(),
                Price = x.price

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
