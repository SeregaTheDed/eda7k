using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class Product
    {
        public int? id { get; set; }
        public int price { get; set; }
        public int category_id { get; set; }
        public int extra { get; set; }
        public string name { get; set; }
        public bool availability_tomorrow { get; set; }
        public bool trash { get; set; }

        public static Product GetEmptyProduct()
        {
            return new Product 
            { 
                id = -1,
                price = 0,
                category_id = 0,
                extra = 0,
                name = "Удаленный продукт",
                availability_tomorrow = false,
            };
        }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
