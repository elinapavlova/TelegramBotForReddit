using System.Collections.Generic;
using System.Net.Http;
using Reddit;
using Reddit.Controllers;
using TelegramBotForReddit.Core.Options;

namespace TelegramBotForReddit.Core.Services.Reddit
{
    public class RedditService : IRedditService
    {
        private readonly string _appRefreshToken;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly IHttpClientFactory _clientFactory;
        
        public RedditService
        (
            AppOptions options, 
            IHttpClientFactory clientFactory
        )
        {
            _appRefreshToken = options.RedditRefreshToken;
            _appId = options.RedditId;
            _appSecret = options.RedditSecret;
            _clientFactory = clientFactory;
        }
        public List<Subreddit> GetSubreddits(string category)
        {
            using var httpclient = _clientFactory.CreateClient("RedditClient");

            var reddit = new RedditClient(_appId, _appRefreshToken, _appSecret);
            
            var subreddits = reddit.GetSubreddits(category);
            return subreddits;
        }
    }
}