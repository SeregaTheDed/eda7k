using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class Order
    {
        public int? id { get; set; }
        public string customer_name { get; set; }
        public DateTime date { get; set; }
        public int user_id { get; set; }
        public int status_id { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}
