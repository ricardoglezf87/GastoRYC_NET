using DAOLib.Models;
using DAOLib.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace DAOLib.Managers
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

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategoryDAO>? vBalancebyCategory { get; set; }

        #endregion


        public RYCContext() : base()
        {
            if (!Settings.Default.BBDDLocal)
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\GastosRYC\\Data\\"))
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\GastosRYC\\Data\\");
            }
            else
            {
                if (!Directory.Exists("Data\\"))
                    Directory.CreateDirectory("Data\\");
            }

            makeBackup();

            Database.Migrate();
        }

        private void makeBackup()
        {
            string path = string.Empty;
            string nameDDBB = string.Empty;

            if (!Settings.Default.BBDDLocal)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\GastosRYC\\Data\\";
            }
            else
            {
                path = "Data\\";
            }

            if (!Directory.Exists(path + "Backup\\"))
                Directory.CreateDirectory(path + "Backup\\");

#if DEBUG
            nameDDBB = "rycBBDD_PRE.db";
#else
            nameDDBB = "rycBBDD.db";
#endif

            if (File.Exists(path + nameDDBB))
            {

                File.Copy(path + nameDDBB, path + "Backup\\" +
                    nameDDBB + "." + DateTime.Now.Ticks.ToString() + ".bk", true);
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

            if (!Settings.Default.BBDDLocal)
            {
                optionsBuilder.UseSqlite("Data Source="
                + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GastosRYC\\Data\\" + nameDDBB);
            }
            else
            {
                optionsBuilder.UseSqlite("Data Source=Data\\" + nameDDBB);
            }
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
