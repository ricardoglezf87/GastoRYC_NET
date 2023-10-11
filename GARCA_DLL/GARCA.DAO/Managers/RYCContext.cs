using GARCA.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace GARCA.DAO.Managers
{
    public class RycContext : DbContext
    {

        #region Tablas

        public DbSet<DateCalendarDao>? DateCalendar { get; set; }
        public DbSet<AccountsDao>? Accounts { get; set; }
        public DbSet<AccountsTypesDao>? AccountsTypes { get; set; }
        public DbSet<TransactionsDao>? Transactions { get; set; }
        public DbSet<TransactionsArchivedDao>? TransactionsArchived { get; set; }
        public DbSet<PersonsDao>? Persons { get; set; }
        public DbSet<CategoriesDao>? Categories { get; set; }
        public DbSet<CategoriesTypesDao>? CategoriesTypes { get; set; }
        public DbSet<TagsDao>? Tags { get; set; }
        public DbSet<TransactionsStatusDao>? TransactionsStatus { get; set; }
        public DbSet<SplitsDao>? Splits { get; set; }
        public DbSet<SplitsArchivedDao>? SplitsArchived { get; set; }
        public DbSet<PeriodsRemindersDao>? PeriodsReminders { get; set; }
        public DbSet<TransactionsRemindersDao>? TransactionsReminders { get; set; }
        public DbSet<SplitsRemindersDao>? SplitsReminders { get; set; }
        public DbSet<ExpirationsRemindersDao>? ExpirationsReminders { get; set; }
        public DbSet<InvestmentProductsDao>? InvestmentProducts { get; set; }
        public DbSet<InvestmentProductsPricesDao>? InvestmentProductsPrices { get; set; }
        public DbSet<InvestmentProductsTypesDao>? InvestmentProductsTypes { get; set; }

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategoryDao>? VBalancebyCategory { get; set; }

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
            modelBuilder.Entity<VBalancebyCategoryDao>()
                .ToView("VBalancebyCategory")
                .HasKey(t => new { year = t.Year, month = t.Month, categoryid = t.Categoryid });
        }

    }
}
