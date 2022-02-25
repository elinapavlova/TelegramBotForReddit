using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserSubscribeModel> UserSubscribes { get; set; }
        public DbSet<AdministratorModel> Administrators { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)         
        {
            Database.EnsureCreatedAsync();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>(user =>
            {
                user.Property(u => u.Id).IsRequired();
                user.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                user.Property(u => u.DateStarted).IsRequired();
            });
            
            builder.Entity<AdministratorModel>(user =>
            {
                user.Property(u => u.UserId).IsRequired();
            });
            
            builder.Entity<AdministratorModel>().HasIndex(a => new {a.UserId}).IsUnique();

            builder.Entity<UserSubscribeModel>(userSubscribe =>
            {
                userSubscribe.Property(us => us.SubredditName).IsRequired().HasMaxLength(100);
                userSubscribe.Property(us => us.DateSubscribed).IsRequired();
                
                userSubscribe.HasOne(us => us.User)
                    .WithMany(u => u.Subscribes)
                    .HasForeignKey(us => us.UserId);
            });
            
            builder.Entity<UserSubscribeModel>().HasIndex(b => new {b.SubredditName, b.UserId}).IsUnique();
        }
    }
}