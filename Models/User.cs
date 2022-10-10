﻿using Microsoft.EntityFrameworkCore;

namespace eda7k.Models
{
    public class User
    {
        public int id { get; set; }
        public bool is_admin { get; set; }
        public string? last_name { get; set; }
        public string login { get; set; }
        
    }
    public partial class DBConnection : DbContext
    {
       /* public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions)
        : base(dbContextOptions)
        {
        }*/
        public DbSet<User> Users { get; set; }
        private static Func<DBConnection, User[]> _getUsers =
            EF.CompileQuery((DBConnection db) =>
                    db.Users.ToArray()
            );
        private static Func<DBConnection,string, User> _getUserByLogin =
            EF.CompileQuery((DBConnection db, string login) =>
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
        
    }
}
