using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class Config
    {
        public int id { get; set; }
        public DateTime next_order_day { get; set; }
        public DateTime last_time_to_do_order { get; set; }
    }

    public partial class DBConnection : DbContext
    {
        private static Func<DBConnection, Task<Config>> _getConfig =
            EF.CompileAsyncQuery( (DBConnection db) =>
                         db.Config.First()
            );
        public async Task<Config> GetConfigAsync()
        {
            return await _getConfig(this);
        } 
        public DbSet<Config> Config { get; set; }
    }
}
