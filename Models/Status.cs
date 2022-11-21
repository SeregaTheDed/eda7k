using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eda7k.Models
{
    public class Status
    {
        [Key]
        public int id { get; private set; }
        public string name { get; private set; }
    }

    public partial class DBConnection : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
    }
}
