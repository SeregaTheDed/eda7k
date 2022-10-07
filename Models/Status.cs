using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class Status
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public partial class DBConnection : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
    }
}
