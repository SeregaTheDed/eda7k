using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class Position
    {
        public int? id { get; set; } 
        public int status_id { get; set; } 
        public DateTime? date { get; set; } //not need in db
        public int product_id_first { get; set; } 
        public int? product_id_second { get; set; } 
        public string? customer_name { get; set; } //not need in db
        public bool with_sauce { get; set; } 
        public int? user_id { get; set; } //not need in db
        public int price { get; set; }
    }
    public partial class DBConnection : DbContext
    {
        public DbSet<Position> Positions { get; set; }

    }
}
