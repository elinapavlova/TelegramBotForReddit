using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Reddit;
using Reddit.Controllers.EventArgs;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Profiles;
using TelegramBotForReddit.Core.Services;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database;
using TelegramBotForReddit.Database.Models.RedditMedia;
using TelegramBotForReddit.Database.Repositories;
using TelegramBotForReddit.Database.Repositories.Contracts;
using ConfigurationBuilder = TelegramBotForReddit.Core.Configurations.ConfigurationBuilder;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TelegramBotForReddit
{
    internal static class Program
    {
        private static IUserSubscribeService _userSubscribeService;
        private static Logger _logger;
        private static ITelegramBotClient _bot;
        private static AppOptions _appOptions;
        private static IConfiguration _configuration;
        private static ITelegramService _telegramService;
        private static IRedditService _redditService;
        private static ISubredditService _subredditService;

        public static async Task Main()
        {
            try
            {
                _configuration = ConfigurationBuilder.Build();
                _logger = NLogBuilder.ConfigureNLog(Path.GetFullPath("nlog.config")).GetCurrentClassLogger();
                
                var services = ConfigureServices();
                
                _appOptions = _configuration.GetSection(AppOptions.App).Get<AppOptions>();
                _telegramService = services.GetService<ITelegramService>();
                _redditService = services.GetService<IRedditService>();
                _userSubscribeService = services.GetService<IUserSubscribeService>();
                _subredditService = services.GetService<ISubredditService>();

                if (_telegramService == null || _redditService == null)
                    throw new Exception("App error: some services not found.");

                using var cts = new CancellationTokenSource();
                
                _bot = _telegramService.CreateBot();
                _bot.StartReceiving(
                    new DefaultUpdateHandler(_telegramService.HandleUpdateAsync, _telegramService.HandleErrorAsync),
                    cts.Token);

                var me = await _bot.GetMeAsync(cts.Token);
                Console.WriteLine($"Start listening for @{me.Username}");
                
                var reddit = new RedditClient(_appOptions.RedditId, _appOptions.RedditRefreshToken, _appOptions.RedditSecret);
                
                foreach (var subreddit in (await _subredditService.GetAll())
                    .Select(sub => reddit.Subreddit(sub.Name)))
                {
                    subreddit.Posts.GetNew();  
                    subreddit.Posts.MonitorNew();
                    subreddit.Posts.NewUpdated += NewPostsUpdated;
                }

                Console.ReadLine();
                cts.Cancel();
            }
            catch (Exception e)
            {
                _logger.Error($"App error: {e.Message}");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static async void NewPostsUpdated(object sender, PostsUpdateEventArgs e)
        {
            foreach (var post in e.Added)
            {
                var users = await _userSubscribeService.GetBySubredditName(post.Subreddit);
                var preview = post.Listing.Preview;
                Media media = null;

                foreach (var user in users)
                {
                    if (preview != null)
                    {
                        media = JsonConvert.DeserializeObject<Media>(preview.ToString());
                        media.Url = _redditService.MakeUrl(post.Listing.Domain, post.Listing.URL, post.Permalink);
                    }

                    await _telegramService.SendMessage(media, user, post);
                }
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var servicesProvider = new ServiceCollection();
            
            var connection = _configuration.GetConnectionString("SQLiteConnection");
            servicesProvider.AddDbContext<AppDbContext>(options => options.UseSqlite(connection,
                x => x.MigrationsAssembly("TelegramBotForReddit.Database")));
            
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AppProfile());
            });
            var mapper = mapperConfig.CreateMapper();

            servicesProvider.AddHttpClient<IRedditService, RedditService>("RedditClient", client =>
            {
                client.BaseAddress = new Uri(_configuration["App:RedditBaseAddress"]);
            });
            
            servicesProvider.AddLogging(logger =>
                {
                    ConfigureExtensions.AddNLog(logger, "nlog.config");
                    logger.AddFilter("Microsoft", LogLevel.Warning);
                })
                .AddSingleton(mapper)
                .Configure<AppOptions>(_configuration.GetSection(AppOptions.App))
                .Configure<CommandsOptions>(_configuration.GetSection(CommandsOptions.Command))
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserSubscribeRepository, UserSubscribeRepository>()
                .AddScoped<IAdministratorRepository, AdministratorRepository>()
                .AddScoped<ISubredditRepository, SubredditRepository>()
                .AddScoped<IRedditService, RedditService>()
                .AddScoped<ITelegramService, TelegramService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IUserSubscribeService, UserSubscribeService>()
                .AddScoped<IAdministratorService, AdministratorService>()
                .AddScoped<ISubredditService, SubredditService>()
                .AddScoped<Commands>();

            return servicesProvider.BuildServiceProvider();
        }
    }
}