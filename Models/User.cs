using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class User
    {
        public int id { get; set; }
        public bool is_admin { get; set; }
        public string? last_name { get; set; }
        public string login { get; set; }
        
    }
    public partial class ApplicationContext : DbContext
    {
       /* public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions)
        : base(dbContextOptions)
        {
        }*/
        public DbSet<User> Users { get; set; }
        private static Func<ApplicationContext, User[]> _getUsers =
            EF.CompileQuery((ApplicationContext db) =>
                    db.Users.ToArray()
            );
        private static Func<ApplicationContext,string, User> _getUserByLogin =
            EF.CompileQuery((ApplicationContext db, string login) =>
                    db.Users.FirstOrDefault(x => x.login == login)
            );
        public User[] getUsers()
        {
            return _getUsers(this);
        }
        public User getUserByLogin(string login)
        {
            return _getUserByLogin(this, login);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = config.GetConnectionString("someecom");
            optionsBuilder.UseSqlServer(connectionString);//M9y-Vqx-Ht3-e8c
        }
    }
}
