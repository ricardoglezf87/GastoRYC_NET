using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
        public DbSet<CategoriesTypes>? categoriesTypes { get; set; }
        public DbSet<Tags>? tags { get; set; }
        public DbSet<TransactionsStatus>? transactionsStatus { get; set; }
        public DbSet<Splits>? splits { get; set; }
        public DbSet<PeriodsReminders>? periodsReminders { get; set; }
        public DbSet<TransactionsReminders>? transactionsReminders { get; set; }
        public DbSet<SplitsReminders>? splitsReminders { get; set; }

        public DbSet<ExpirationsReminders>? expirationsReminders { get; set; }

        public RYCContext() : base()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\GastosRYC\\Data\\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\GastosRYC\\Data\\");

            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder.UseSqlite("Data Source="
                + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\GastosRYC\\Data\\rycBBDD.db");

    }
}
