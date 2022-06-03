using System.Threading.Tasks;
using TelegramBotForReddit.Core.Services;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Handlers
{
    public class SubredditHandler
    {
        private readonly ISubredditService _subredditService;
        private readonly IRedditService _redditService;

        public SubredditHandler(ISubredditService subredditService, IRedditService redditService)
        {
            _subredditService = subredditService;
            _redditService = redditService;
        }
        
        public async Task AddSubreddit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            
            var isSubredditExist = await _redditService.IsSubredditExist(name);
            if ((bool)isSubredditExist)
            {
                await _subredditService.Create(name);
                RedditBotService.SubredditsName.Add(name);
            }
        }
    }
}