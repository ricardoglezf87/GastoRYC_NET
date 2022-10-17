using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BBDDLib.Manager
{
    public class RYCContext : DbContext
    {
        public DbSet<Accounts>? accounts { get; set; }
        public DbSet<AccountsTypes>? accountsTypes { get; set; }
        public DbSet<Transactions>? transactions { get; set; }
        public DbSet<Persons>? persons { get; set; }
        public DbSet<Categories>? categories { get; set; }

        private string getPathDDBB()
        {
#if DEBUG
            return "C:\\Users\\Ricardo\\source\\repos\\GastosRYC_ASP\\BBDDLib\\BBDD\\test.mdf";
#else
             return "C:\\Users\\Ricardo\\source\\repos\\GastosRYC_ASP\\BBDDLib\\BBDD\\Data.mdf";
#endif
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename="+ getPathDDBB() + ";Integrated Security=True");

    }
}
