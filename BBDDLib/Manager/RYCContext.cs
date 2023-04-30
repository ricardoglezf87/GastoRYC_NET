using BBDDLib.Models;
using BBDDLib.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BBDDLib.Manager
{
    public class RYCContext : DbContext
    {

        #region Tablas

        public DbSet<DateCalendar>? dateCalendar { get; set; }
        public DbSet<Accounts>? accounts { get; set; }
        public DbSet<AccountsTypes>? accountsTypes { get; set; }
        public DbSet<Transactions>? transactions { get; set; }
        public DbSet<Persons>? persons { get; set; }
        public DbSet<Categories>? categories { get; set; }
        public DbSet<CategoriesTypes>? categoriesTypes { get; set; }
        public DbSet<Tags>? tags { get; set; }
        public DbSet<TransactionsStatus>? transactionsStatus { get; set; }
        public DbSet<Splits>? splits { get; set; }
        public DbSet<PeriodsReminders>? periodsReminders { get; set; }
        public DbSet<TransactionsReminders>? transactionsReminders { get; set; }
        public DbSet<SplitsReminders>? splitsReminders { get; set; }
        public DbSet<ExpirationsReminders>? expirationsReminders { get; set; }

        #endregion

        #region Vistas

        public DbSet<VBalancebyCategory>? vBalancebyCategory { get; set; }

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
            String path = String.Empty;
            String nameDDBB = String.Empty;

            if (!Settings.Default.BBDDLocal)
            {
                path =Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
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

            if(File.Exists(path + nameDDBB)) {
                
                File.Copy(path + nameDDBB, path + "Backup\\" + 
                    nameDDBB + "." + DateTime.Now.Ticks.ToString()+".bk",true);
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            String nameDDBB = String.Empty;

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
            modelBuilder.Entity<VBalancebyCategory>()
                .ToView(nameof(VBalancebyCategory))
                .HasKey(t => new { t.year, t.month, t.categoryid });
        }

    }
}
