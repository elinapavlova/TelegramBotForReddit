using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserSubscribeModel> UserSubscribes { get; set; }
        public DbSet<AdministratorModel> Administrators { get; set; }
        public DbSet<SubredditModel> Subreddits { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)         
        {
            Database.EnsureCreatedAsync();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>(_ =>
            {
                _.Property(user => user.Id).IsRequired();
                _.Property(user => user.UserName).HasMaxLength(50);
                _.Property(user => user.DateStarted).IsRequired();
            });
            
            builder.Entity<AdministratorModel>(_ =>
            {
                _.Property(admin => admin.UserId).IsRequired();
                _.Property(admin => admin.IsSuperAdmin).IsRequired().HasDefaultValue(false);
            });
            
            builder.Entity<AdministratorModel>().HasIndex(admin => new {admin.UserId}).IsUnique();

            builder.Entity<UserSubscribeModel>(_ =>
            {
                _.Property(userSubscribe => userSubscribe.SubredditName).IsRequired().HasMaxLength(100);
                _.Property(userSubscribe => userSubscribe.DateSubscribed).IsRequired();
                
                _.HasOne(us => us.User)
                    .WithMany(u => u.Subscribes)
                    .HasForeignKey(us => us.UserId);
            });
            
            builder.Entity<UserSubscribeModel>().HasIndex(userSubscribe => new {userSubscribe.SubredditName, userSubscribe.UserId}).IsUnique();
            
            builder.Entity<SubredditModel>(_ =>
            {
                _.Property(subreddit => subreddit.Id).IsRequired();
                _.Property(subreddit => subreddit.Name).IsRequired().HasMaxLength(50);
            });
            builder.Entity<SubredditModel>().HasIndex(subreddit => new {subreddit.Name}).IsUnique();
        }
    }
}