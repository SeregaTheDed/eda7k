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
        public async Task<IActionResult> AskActionLongPool([FromBody] int id)
        {
            var result = await MyChannel.GetSheduleTask();//Ожидание пока кто-то что-то не купит
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> BuyPositionById([FromBody] int id)
        {
            using (var db = new DBConnection())
            {
                var buyingPosition = await db.Positions.FirstOrDefaultAsync(x => x.id == id);
                if (buyingPosition == null) 
                {
                    return BadRequest();
                }
                else
                {
                    buyingPosition.user_id = _user.id;
                    buyingPosition.date = DateTime.Now;
                    buyingPosition.customer_name = _user.last_name;
                    await db.SaveChangesAsync();
                    await MyChannel.WriteAsync(new KeyValuePair<Operations, int>
                       (
                       Operations.Delete,
                       id
                       ));
                    return Ok();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPositionById([FromBody] int id)
        {
            using (var db = new DBConnection())
            {
                var findingPosition = await db.Positions.FirstOrDefaultAsync(x => x.id == id);
                if (findingPosition == null)
                    return NotFound();
                return new OkObjectResult(findingPosition);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePositionStatusById(int id, int newStatusId)
        {
            if (_user.is_admin == false)
                return NotFound();
            using (DBConnection db = new())
            {
                var needPosition = await db.Positions.FirstAsync(x => x.id == id);
                needPosition.status_id = newStatusId;
                await db.SaveChangesAsync();
                return Ok();
            }
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

                var PositionViews = Positions
                    .Select(x =>  GetPositionViewFromPosition(x, ProductsById));

                return new OkObjectResult(Positions);
            }
        }

        private static PositionView GetPositionViewFromPosition(Position position, Dictionary<int, Product> ProductsById)
        {
            StringBuilder productNameSB = new StringBuilder();

            Product product1 = ProductsById[position.product_id_first];
            productNameSB.Append(product1.name);

            if (position.product_id_second.HasValue)
            {
                Product product2;
                if (ProductsById.ContainsKey(position.product_id_second.Value))
                {
                    product2 = ProductsById[position.product_id_second.Value];
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
