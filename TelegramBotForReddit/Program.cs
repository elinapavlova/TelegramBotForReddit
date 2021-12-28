using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Reddit;
using Reddit.Controllers.EventArgs;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Telegram;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit
{
    static class Program
    {
        private static IUserSubscribeService _userSubscribeService;
        private static Logger _logger;
        private static ITelegramBotClient _bot;
        private static AppOptions _appOptions;
        
        public static async Task Main()
        {
            const string path = "";
            _logger = NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();

            try
            {
                var startup = new Startup();
                IServiceCollection services = new ServiceCollection();
                startup.ConfigureServices(services);

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                var telegramService = serviceProvider.GetService<ITelegramService>();
                _userSubscribeService = serviceProvider.GetService<IUserSubscribeService>();
                _appOptions = serviceProvider.GetService<AppOptions>();
                _bot = telegramService.CreateBot();

                using var cts = new CancellationTokenSource();
                
                _bot.StartReceiving(
                    new DefaultUpdateHandler(telegramService.HandleUpdateAsync, telegramService.HandleErrorAsync),
                    cts.Token);

                var me = await _bot.GetMeAsync(cts.Token);
                Console.WriteLine($"Start listening for @{me.Username}");
                
                var reddit = new RedditClient(_appOptions.RedditId, _appOptions.RedditRefreshToken, _appOptions.RedditSecret);

                foreach (var subreddit in reddit.GetSubreddits("popular")
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
                _logger.Error(e, "Stopped program because of exception");
                Console.WriteLine(e.Message);
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

                foreach (var user in users)
                    await _bot.SendTextMessageAsync(user.UserId, 
                        $"{post.Subreddit}\r\n" +
                        $"{post.Created.ToLocalTime()}\r\n" +
                        $"{post.Title}\r\n" +
                        $"{_appOptions.RedditBaseAddress}{post.Permalink}");
            }
        }
    }
}