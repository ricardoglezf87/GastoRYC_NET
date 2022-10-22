using BBDDLib.Helpers;
using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Windows;

namespace BBDDLib.Manager
{
    public class RYCContext : DbContext
    {
        public DbSet<Accounts>? accounts { get; set; }
        public DbSet<AccountsTypes>? accountsTypes { get; set; }
        public DbSet<Transactions>? transactions { get; set; }
        public DbSet<Persons>? persons { get; set; }
        public DbSet<Categories>? categories { get; set; }
        public DbSet<TransactionsStatus>? transactionsStatus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ PathHelpers.getPathDDBB() + ";Integrated Security=True");

    }
}
