using GARCA.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GARCA.DAO.Managers
{
    public class RYCContext : DbContext
    {

        #region Tablas

        public DbSet<DateCalendarDAO>? dateCalendar { get; set; }
        public DbSet<AccountsDAO>? accounts { get; set; }
        public DbSet<AccountsTypesDAO>? accountsTypes { get; set; }
        public DbSet<TransactionsDAO>? transactions { get; set; }
        public DbSet<PersonsDAO>? persons { get; set; }
        public DbSet<CategoriesDAO>? categories { get; set; }
        public DbSet<CategoriesTypesDAO>? categoriesTypes { get; set; }
        public DbSet<TagsDAO>? tags { get; set; }
        public DbSet<TransactionsStatusDAO>? transactionsStatus { get; set; }
        public DbSet<SplitsDAO>? splits { get; set; }
        public DbSet<PeriodsRemindersDAO>? periodsReminders { get; set; }
        public DbSet<TransactionsRemindersDAO>? transactionsReminders { get; set; }
        public DbSet<SplitsRemindersDAO>? splitsReminders { get; set; }
        public DbSet<ExpirationsRemindersDAO>? expirationsReminders { get; set; }
        public DbSet<InvestmentProductsDAO>? investmentProducts { get; set; }
        public DbSet<InvestmentProductsPricesDAO>? investmentProductsPrices { get; set; }
        public DbSet<InvestmentProductsTypesDAO>? investmentProductsTypes { get; set; }

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategoryDAO>? vBalancebyCategory { get; set; }

        #endregion

        public RYCContext() : base()
        {
            if (!Directory.Exists("Data\\"))
            {
                Directory.CreateDirectory("Data\\");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string nameDDBB = string.Empty;

#if DEBUG
            nameDDBB = "rycBBDD_PRE.db";
#else
            nameDDBB = "rycBBDD.db";
#endif

            optionsBuilder.UseSqlite("Data Source=Data\\" + nameDDBB);

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
