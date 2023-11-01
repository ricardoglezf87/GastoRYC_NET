using GARCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GARCA.Data.Managers
{
    public class RycContext : DbContext
    {

        #region Tablas

        public DbSet<DateCalendar>? DateCalendar { get; set; }
        public DbSet<Accounts>? Accounts { get; set; }
        public DbSet<AccountsTypes>? AccountsTypes { get; set; }
        public DbSet<Transactions>? Transactions { get; set; }
        public DbSet<TransactionsArchived>? TransactionsArchived { get; set; }
        public DbSet<Persons>? Persons { get; set; }
        public DbSet<Categories>? Categories { get; set; }
        public DbSet<CategoriesTypes>? CategoriesTypes { get; set; }
        public DbSet<Tags>? Tags { get; set; }
        public DbSet<TransactionsStatus>? TransactionsStatus { get; set; }
        public DbSet<Splits>? Splits { get; set; }
        public DbSet<SplitsArchived>? SplitsArchived { get; set; }
        public DbSet<PeriodsReminders>? PeriodsReminders { get; set; }
        public DbSet<TransactionsReminders>? TransactionsReminders { get; set; }
        public DbSet<SplitsReminders>? SplitsReminders { get; set; }
        public DbSet<ExpirationsReminders>? ExpirationsReminders { get; set; }
        public DbSet<InvestmentProducts>? InvestmentProducts { get; set; }
        public DbSet<InvestmentProductsPrices>? InvestmentProductsPrices { get; set; }
        public DbSet<InvestmentProductsTypes>? InvestmentProductsTypes { get; set; }

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategory>? VBalancebyCategory { get; set; }

        #endregion

        public RycContext()
        {
            if (!Directory.Exists("Data\\"))
            {
                Directory.CreateDirectory("Data\\");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var nameDdbb = string.Empty;

#if DEBUG
            nameDdbb = "rycBBDD_PRE.db";
#else
            nameDdbb = "rycBBDD.db";
#endif

            optionsBuilder.UseSqlite("Data Source=Data\\" + nameDdbb);

            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VBalancebyCategory>()
                .ToView("VBalancebyCategory")
                .HasKey(t => new { year = t.Year, month = t.Month, categoryid = t.Categoryid });
        }

    }
}
