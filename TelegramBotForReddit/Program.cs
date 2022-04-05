using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Profiles;
using TelegramBotForReddit.Core.Services;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database;
using TelegramBotForReddit.Database.Repositories;
using TelegramBotForReddit.Database.Repositories.Contracts;
using ConfigurationBuilder = TelegramBotForReddit.Core.Configurations.ConfigurationBuilder;
using Logger = TelegramBotForReddit.Core.Logger.Logger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TelegramBotForReddit
{
    internal static class Program
    {
        private static IConfiguration _configuration;
        private static IRedditBotService _redditBotService;

        public static void Main()
        {
            try
            {
                _configuration = ConfigurationBuilder.Build();

                var services = ConfigureServices();
                
                _redditBotService = services.GetService<IRedditBotService>();
                if (_redditBotService is null)
                    throw new Exception("App error: RedditBotService not found.");

                _redditBotService.Work();
            }
            catch (Exception e)
            {
                Logger.LogError($"App error: {e.Message}");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var servicesProvider = new ServiceCollection();

            servicesProvider.AddLogging(logger =>
                {
                    ConfigureExtensions.AddNLog(logger, "nlog.config");
                    logger.AddFilter("Microsoft", LogLevel.Warning);
                })
                .AddSingleton(ConfigureMapper())
                .Configure<AppOptions>(_configuration.GetSection(AppOptions.App))
                .Configure<CommandsOptions>(_configuration.GetSection(CommandsOptions.Command))
                .Configure<RedditOptions>(_configuration.GetSection(RedditOptions.Reddit))
                .AddSingleton<CommandList>();
            
            AddDbContext(servicesProvider);
            AddHttpClients(servicesProvider);
            AddRepositoryInjections(servicesProvider);
            AddServiceInjections(servicesProvider);

            return servicesProvider.BuildServiceProvider();
        }
        
        private static IMapper ConfigureMapper()
        {
            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new AppProfile()); });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }

        private static void AddDbContext(IServiceCollection servicesProvider)
        {
            var connection = _configuration.GetConnectionString("SQLiteConnection");
            servicesProvider.AddDbContext<AppDbContext>(options => options.UseSqlite(connection,
                x => x.MigrationsAssembly("TelegramBotForReddit.Database")));
        }
        
        private static void AddHttpClients(IServiceCollection servicesProvider)
        {
            servicesProvider.AddHttpClient<RedditHttpClient>("RedditClient",
                client => { client.BaseAddress = new Uri(_configuration["Reddit:BaseAddress"]); });
            
            servicesProvider.AddScoped<RedditHttpClient>();
            servicesProvider.AddScoped<TelegramHttpClient>();
        }

        private static void AddRepositoryInjections(IServiceCollection servicesProvider)
        {
            servicesProvider
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserSubscribeRepository, UserSubscribeRepository>()
                .AddScoped<IAdministratorRepository, AdministratorRepository>()
                .AddScoped<ISubredditRepository, SubredditRepository>();
        }
        
        private static void AddServiceInjections(IServiceCollection servicesProvider)
        {
            servicesProvider
                .AddScoped<IRedditService, RedditService>()
                .AddScoped<ITelegramService, TelegramService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IUserSubscribeService, UserSubscribeService>()
                .AddScoped<IAdministratorService, AdministratorService>()
                .AddScoped<ISubredditService, SubredditService>()
                .AddScoped<IRedditBotService, RedditBotService>();
        }
    }
}