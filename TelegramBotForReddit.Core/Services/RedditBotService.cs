using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using Telegram.Bot;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models.RedditMedia;

namespace TelegramBotForReddit.Core.Services
{
    public class RedditBotService : IRedditBotService
    {
        private static IUserSubscribeService _userSubscribeService;
        private static ITelegramBotClient _bot;
        private static TelegramHttpClient _telegramHttpClient;
        private static ITelegramService _telegramService;
        private static RedditHttpClient _redditHttpClient;
        private static IRedditService _redditService;
        private readonly ISubredditService _subredditService;
        private readonly ISmtpSender _smtpSender;

        public RedditBotService
        (
            IUserSubscribeService userSubscribeService,
            TelegramHttpClient telegramHttpClient,
            RedditHttpClient redditHttpClient,
            ITelegramService telegramService,
            IRedditService redditService,
            ISubredditService subredditService,
            ISmtpSender smtpSender
        )
        {
            _userSubscribeService = userSubscribeService;
            _telegramHttpClient = telegramHttpClient;
            _redditHttpClient = redditHttpClient;
            _telegramService = telegramService;
            _redditService = redditService;
            _subredditService = subredditService;
            _smtpSender = smtpSender;
        }

        public async Task Work()
        {
            try
            {
                CheckServicesExist();

                using var cts = new CancellationTokenSource();
                InitializeBot(cts);
                Console.WriteLine($"{DateTime.Now} : Bot started.");
                Logger.Logger.LogInfo($"Bot started {DateTime.Now}");

                CheckNewPosts();

                Console.ReadLine();
                cts.Cancel();
            }
            catch (Exception e)
            {
                Logger.Logger.LogError($"App error: {e.Message}");
                await _smtpSender.SendMessage(e.Message);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private void CheckServicesExist()
        {
            if (_telegramHttpClient is null || _telegramService is null || _redditService is null
                || _subredditService is null || _redditHttpClient is null)
                throw new Exception("App error: some services not found.");
        }
        
        private static void InitializeBot(CancellationTokenSource cts)
        {
            _bot = _telegramHttpClient.CreateBot();
            _bot.StartReceiving(_telegramService.CreateDefaultUpdateHandler(), cts.Token);
        }
        
        private static void CheckNewPosts()
        {
            var reddit = _redditHttpClient.CreateRedditClient();
            var subreddits = _redditService.GetSubreddits("popular");
            
            foreach (var subreddit in subreddits.Select(subreddit => reddit.Subreddit(subreddit.Name)))
            {
                subreddit.Posts.GetNew();
                subreddit.Posts.MonitorNew();
                subreddit.Posts.NewUpdated += NewPostsUpdated;
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
                        media = CreateMedia(preview, post);

                    await _telegramService.SendMessage(media, user, post);
                }
            }
        }

        private static Media CreateMedia(JObject preview, Post post)
        {
            var media = JsonConvert.DeserializeObject<Media>(preview.ToString());
            media.Url = _redditService.MakeUrl(post.Listing.Domain, post.Listing.URL, post.Permalink);
            return media;
        }
    }
}