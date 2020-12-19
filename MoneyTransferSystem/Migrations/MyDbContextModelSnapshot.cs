﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneyTransferSystem.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoneyTransferSystem.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Money")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("CharCode")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CharCode")
                        .IsUnique();

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CharCode = "USD",
                            Name = "USA dollar"
                        });
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.GlobalMoneyRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<decimal>("Comission")
                        .HasColumnType("numeric");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Max")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Min")
                        .HasColumnType("numeric");

                    b.Property<bool>("isComissionFixed")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.ToTable("GlobalMoneyRules");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.MoneyRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<decimal>("Comission")
                        .HasColumnType("numeric");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Max")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Min")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<bool>("isComissionFixed")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("MoneyRules");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.Property<string>("Pass")
                        .HasColumnType("text");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Login = "Adam",
                            Pass = "123",
                            isAdmin = false
                        });
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.Account", b =>
                {
                    b.HasOne("MoneyTransferSystem.Database.DbModels.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId");

                    b.HasOne("MoneyTransferSystem.Database.DbModels.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.GlobalMoneyRule", b =>
                {
                    b.HasOne("MoneyTransferSystem.Database.DbModels.Currency", "Currency")
                        .WithMany("GlobalMoneyRules")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.MoneyRule", b =>
                {
                    b.HasOne("MoneyTransferSystem.Database.DbModels.Currency", "Currency")
                        .WithMany("MoneyRules")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneyTransferSystem.Database.DbModels.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.Currency", b =>
                {
                    b.Navigation("GlobalMoneyRules");

                    b.Navigation("MoneyRules");
                });

            modelBuilder.Entity("MoneyTransferSystem.Database.DbModels.User", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
