using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public partial class DBConnection : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = config.GetConnectionString("someecom");
            optionsBuilder.UseSqlServer(connectionString);//M9y-Vqx-Ht3-e8c
            optionsBuilder.LogTo(System.Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name });
        }
    }
}
