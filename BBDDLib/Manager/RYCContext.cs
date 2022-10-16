using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Manager
{
    public class RYCContext : DbContext
    {
        public DbSet<Accounts>? accounts { get; set; }
        public DbSet<AccountsTypes>? accountsTypes { get; set; }
        public DbSet<Transactions>? transactions { get; set; }
        public DbSet<Persons>? persons { get; set; }
        public DbSet<Categories>? categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder.UseNpgsql("Host=gastosryc.c4ajgiqk32b0.us-east-1.rds.amazonaws.com;Port=5405;Database=postgres;Pooling=true;SearchPath=ryc_test;Username=rgonzalez;Password=uci7wXFEDMI0WqpEs7MW");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //    modelBuilder.Entity<Item>(entity =>
            //    {
            //        entity.ToTable("item", "account");
            //        entity.Property(e => e.Id)
            //                            .HasColumnName("id")
            //                            .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");
            //        entity.Property(e => e.Description).HasColumnName("description");
            //        entity.Property(e => e.Name)
            //                            .IsRequired()
            //                            .HasColumnName("name");
            //    });
            //    modelBuilder.HasSequence("item_id_seq", "account");

            //modelBuilder.Entity<AccountsTypes>()
            //.HasMany(i => i.accounts)
            //.WithOne(e => e.accountsTypes);


            //modelBuilder.Entity<Accounts>()
            //.HasOne(i => i.accountsTypes)
            //.WithMany(c => c.accounts);

        }
 
    }
}
