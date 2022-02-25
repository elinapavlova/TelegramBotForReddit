using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Reddit;
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
            IOptions<AppOptions> options, 
            IHttpClientFactory clientFactory
        )
        {
            _appRefreshToken = options.Value.RedditRefreshToken;
            _appId = options.Value.RedditId;
            _appSecret = options.Value.RedditSecret;
            _clientFactory = clientFactory;
        }
        public List<global::Reddit.Controllers.Subreddit> GetSubreddits(string category)
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
    }
}