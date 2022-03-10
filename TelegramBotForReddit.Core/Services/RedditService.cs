using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reddit;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class RedditService : IRedditService
    {
        private readonly string _appRefreshToken;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly IHttpClientFactory _clientFactory;
        
        public RedditService
        (
            IOptions<AppOptions> options, 
            IHttpClientFactory clientFactory
        )
        {
            _appRefreshToken = options.Value.RedditRefreshToken;
            _appId = options.Value.RedditId;
            _appSecret = options.Value.RedditSecret;
            _clientFactory = clientFactory;
        }
        public IEnumerable<Reddit.Controllers.Subreddit> GetSubreddits(string category)
        {
            using var httpclient = _clientFactory.CreateClient("RedditClient");

            var reddit = new RedditClient(_appId, _appRefreshToken, _appSecret);
            
            var subreddits = reddit.GetSubreddits(category);
            return subreddits;
        }
        
        // Если видео загружено на Reddit - перенаправлять на сервис для показа видео, иначе вернуть ссылку
        public string MakeUrl(string domain, string url, string permalink)
            => domain == "v.redd.it" 
                ? $"https://vrddit.com{permalink}" 
                : url;

        public async Task<bool?> IsSubredditExist(string name)
        {
            if (name == null)
                return null;
            
            using var httpclient = _clientFactory.CreateClient("RedditClient");

            var response = await httpclient.GetAsync($"r/{name}.json");
            return response.IsSuccessStatusCode;
        }
    }
}