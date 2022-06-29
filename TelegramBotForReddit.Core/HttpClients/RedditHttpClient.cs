using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reddit;
using TelegramBotForReddit.Core.Options;

namespace TelegramBotForReddit.Core.HttpClients
{
    public class RedditHttpClient
    {
        private readonly string _appRefreshToken;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly IHttpClientFactory _clientFactory;
        
        public RedditHttpClient
        (
            IOptions<RedditOptions> options, 
            IHttpClientFactory clientFactory
        )
        {
            _appRefreshToken = options.Value.RefreshToken;
            _appId = options.Value.Id;
            _appSecret = options.Value.Secret;
            _clientFactory = clientFactory;
        }

        public RedditClient CreateRedditClient()
            => new(_appId, _appRefreshToken, _appSecret);

        public IEnumerable<Reddit.Controllers.Subreddit> GetSubredditsByCategory(string category)
        {
            var reddit = new RedditClient(_appId, _appRefreshToken, _appSecret);
            var subreddits = reddit.GetSubreddits(category);
            return subreddits;
        }

        public async Task<bool?> IsSubredditExist(string name)
        {
            using var httpclient = _clientFactory.CreateClient("RedditClient");

            var response = await httpclient.GetAsync($"r/{name}.json");
            return response.IsSuccessStatusCode;
        }
    }
}