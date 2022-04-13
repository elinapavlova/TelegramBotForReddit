using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public static async Task Main()
        {
            try
            {
                _configuration = ConfigurationBuilder.Build();

                var services = new ServiceCollection();
                ConfigureServices(services);
                await using var provider = services.BuildServiceProvider();

                await MigrationUp(provider);

                _redditBotService = provider.GetService<IRedditBotService>();
                if (_redditBotService is null)
                    throw new Exception("App error: RedditBotService not found.");

                await _redditBotService.Work();
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

        private static async Task MigrationUp(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetService<AppDbContext>();
            await context.Database.MigrateAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logger =>
                {
                    ConfigureExtensions.AddNLog(logger, "nlog.config");
                    logger.AddFilter("Microsoft", LogLevel.Warning);
                })
                .AddSingleton(ConfigureMapper())
                .Configure<AppOptions>(_configuration.GetSection(AppOptions.App))
                .Configure<CommandsOptions>(_configuration.GetSection(CommandsOptions.Command))
                .Configure<RedditOptions>(_configuration.GetSection(RedditOptions.Reddit))
                .AddSingleton<CommandList>();
            
            AddDbContext(services);
            AddHttpClients(services);
            AddRepositoryInjections(services);
            AddServiceInjections(services);
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