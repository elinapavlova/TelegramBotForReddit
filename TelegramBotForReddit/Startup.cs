using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Profiles;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.Telegram;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;
using TelegramBotForReddit.Database;
using TelegramBotForReddit.Database.Repositories.User;
using TelegramBotForReddit.Database.Repositories.UserSubscribe;
using ConfigurationBuilder = TelegramBotForReddit.Core.Configurations.ConfigurationBuilder;

namespace TelegramBotForReddit
{
    public class Startup
    {
        public Startup()
        {
            Configuration = ConfigurationBuilder.Build();
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppOptions>(Configuration.GetSection(AppOptions.App));
            var appOptions = Configuration.GetSection(AppOptions.App).Get<AppOptions>();
            services.AddSingleton(appOptions);
            
            services.Configure<CommandsOptions>(Configuration.GetSection(CommandsOptions.Command));
            var commandsOptions = Configuration.GetSection(CommandsOptions.Command).Get<CommandsOptions>();
            services.AddSingleton(commandsOptions);

            services.AddHttpClient<IRedditService, RedditService>("RedditClient", client =>
            {
                client.BaseAddress = new Uri(appOptions.RedditBaseAddress);
            });
            
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection,  
                x => x.MigrationsAssembly("TelegramBotForReddit.Database")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSubscribeRepository, UserSubscribeRepository>();

            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<IRedditService, RedditService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserSubscribeService, UserSubscribeService>();
        }
    }
}