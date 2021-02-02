using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Helpers;

namespace MoneyTransferSystem.Database
{
    public class MyDbContext:DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<User>Users { get; set; }
        public DbSet<Account>Accounts { get; set; }
        public DbSet<Currency>Currencies { get; set; }
        public DbSet<GlobalCommission>GlobalMoneyRules { get; set; }
        public DbSet<PersonalCommission>MoneyRules { get; set; }
        public DbSet<Transfer>Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.ApplySnakeCase(builder);
            this.ApplyOnModelCreatingFromAllEntities(builder);
            
            builder.Entity<Currency>()
                .HasIndex(x => x.CharCode).IsUnique();

            builder.Entity<User>()
                .HasIndex(x => x.Login).IsUnique();

            builder.Entity<Transfer>(modelBuilder =>
            {
                modelBuilder.Property(t => t.Type)
                    .HasConversion<string>();
                modelBuilder.Property(t => t.Status)
                    .HasConversion<string>();
            });
        }

    }
}