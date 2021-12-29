﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBotForReddit.Database;

namespace TelegramBotForReddit.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211228131502_InitSqlite")]
    partial class InitSqlite
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.UserModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.UserSubscribeModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateSubscribed")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateUnsubscribed")
                        .HasColumnType("TEXT");

                    b.Property<string>("SubredditName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SubredditName")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("UserSubscribes");
                });

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.UserSubscribeModel", b =>
                {
                    b.HasOne("TelegramBotForReddit.Database.Models.UserModel", "User")
                        .WithMany("Subscribes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.UserModel", b =>
                {
                    b.Navigation("Subscribes");
                });
#pragma warning restore 612, 618
        }
    }
}
