using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    [Keyless]
    public class Config
    {
        public DateTime next_order_day { get; set; }
        public DateTime last_time_to_do_order { get; set; }
    }

    public partial class DBConnection : DbContext
    {
        public DbSet<Config> Config { get; set; }
    }
}
