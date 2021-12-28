using System.Collections.Generic;
using Reddit.Controllers;

namespace TelegramBotForReddit.Core.Services.Reddit
{
    public interface IRedditService
    {
        List<Subreddit> GetSubreddits(string category);
    }
}