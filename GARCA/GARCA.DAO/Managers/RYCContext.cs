using GARCA.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GARCA.DAO.Managers
{
    public class RycContext : DbContext
    {

        #region Tablas

        public DbSet<DateCalendarDAO>? DateCalendar { get; set; }
        public DbSet<AccountsDAO>? Accounts { get; set; }
        public DbSet<AccountsTypesDAO>? AccountsTypes { get; set; }
        public DbSet<TransactionsDAO>? Transactions { get; set; }
        public DbSet<PersonsDAO>? Persons { get; set; }
        public DbSet<CategoriesDAO>? Categories { get; set; }
        public DbSet<CategoriesTypesDAO>? CategoriesTypes { get; set; }
        public DbSet<TagsDAO>? Tags { get; set; }
        public DbSet<TransactionsStatusDAO>? TransactionsStatus { get; set; }
        public DbSet<SplitsDAO>? Splits { get; set; }
        public DbSet<PeriodsRemindersDAO>? PeriodsReminders { get; set; }
        public DbSet<TransactionsRemindersDAO>? TransactionsReminders { get; set; }
        public DbSet<SplitsRemindersDAO>? SplitsReminders { get; set; }
        public DbSet<ExpirationsRemindersDAO>? ExpirationsReminders { get; set; }
        public DbSet<InvestmentProductsDAO>? InvestmentProducts { get; set; }
        public DbSet<InvestmentProductsPricesDAO>? InvestmentProductsPrices { get; set; }
        public DbSet<InvestmentProductsTypesDAO>? InvestmentProductsTypes { get; set; }

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategoryDAO>? VBalancebyCategory { get; set; }

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
            nameDDBB = "rycBBDD.db";
#endif

            optionsBuilder.UseSqlite("Data Source=Data\\" + nameDdbb);

            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VBalancebyCategoryDAO>()
                .ToView("VBalancebyCategory")
                .HasKey(t => new { t.year, t.month, t.categoryid });
        }

    }
}
