using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class RedditService : IRedditService
    {
        private readonly RedditHttpClient _redditHttpClient;
        
        public RedditService(RedditHttpClient redditHttpClient)
        {
            _redditHttpClient = redditHttpClient;
        }
        
        public IEnumerable<Reddit.Controllers.Subreddit> GetSubreddits(string category)
            => _redditHttpClient.GetSubredditsByCategory(category);

        // Если видео загружено на Reddit - перенаправлять на сервис для показа видео, иначе вернуть новую ссылку
        public string MakeUrl(string domain, string url, string permalink)
            => domain == "v.redd.it" 
                ? $"https://vrddit.com{permalink}" 
                : url;

        public async Task<bool?> IsSubredditExist(string name)
        {
            if (name == null)
                return null;

            return await _redditHttpClient.IsSubredditExist(name);
        }
    }
}