using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.Controllers;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface IRedditService
    {
        IEnumerable<Subreddit> GetSubreddits(string category);
        string MakeUrl(string domain, string url, string permalink);
        Task<bool?> IsSubredditExist(string name);
    }
}