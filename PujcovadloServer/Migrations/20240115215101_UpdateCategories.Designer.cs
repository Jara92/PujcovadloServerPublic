﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PujcovadloServer.data;

#nullable disable

namespace PujcovadloServer.Migrations
{
    [DbContext(typeof(PujcovadloServerContext))]
    [Migration("20240115215101_UpdateCategories")]
    partial class UpdateCategories
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ItemItemCategory", b =>
                {
                    b.Property<int>("ItemCategoriesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemCategoriesId", "ItemsId");

                    b.HasIndex("ItemsId");

                    b.ToTable("ItemItemCategory");
                });

            modelBuilder.Entity("PujcovadloServer.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Alias")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Parameters")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("PricePerDay")
                        .HasColumnType("REAL");

                    b.Property<float?>("PurchasePrice")
                        .HasColumnType("REAL");

                    b.Property<float?>("RefundableDeposit")
                        .HasColumnType("REAL");

                    b.Property<float?>("SellingPrice")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("PujcovadloServer.Models.ItemCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("ItemCategory");
                });

            modelBuilder.Entity("ItemItemCategory", b =>
                {
                    b.HasOne("PujcovadloServer.Models.ItemCategory", null)
                        .WithMany()
                        .HasForeignKey("ItemCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PujcovadloServer.Models.Item", null)
                        .WithMany()
                        .HasForeignKey("ItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PujcovadloServer.Models.ItemCategory", b =>
                {
                    b.HasOne("PujcovadloServer.Models.ItemCategory", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });
#pragma warning restore 612, 618
        }
    }
}
