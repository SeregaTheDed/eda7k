using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    [Keyless]
    public class Rel_orders_product
    {
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int count { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Rel_orders_product> Rel_orders_products { get; set; }
    }
}
