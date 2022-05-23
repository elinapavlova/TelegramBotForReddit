﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBotForReddit.Database;

namespace TelegramBotForReddit.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.14");

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.AdministratorModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSuperAdmin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.SubredditModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Subreddits");
                });

            modelBuilder.Entity("TelegramBotForReddit.Database.Models.UserModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateStarted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateStopped")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
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

                    b.HasIndex("UserId");

                    b.HasIndex("SubredditName", "UserId")
                        .IsUnique();

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
