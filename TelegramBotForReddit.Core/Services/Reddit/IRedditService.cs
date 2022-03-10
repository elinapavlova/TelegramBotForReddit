using System.Collections.Generic;
using System.Threading.Tasks;


namespace TelegramBotForReddit.Core.Services.Reddit
{
    public interface IRedditService
    {
        IEnumerable<global::Reddit.Controllers.Subreddit> GetSubreddits(string category);
        string MakeUrl(string domain, string url, string permalink);
        Task<bool?> IsSubredditExist(string name);
    }
}