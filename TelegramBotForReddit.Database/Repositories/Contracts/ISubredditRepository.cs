using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.Contracts
{
    public interface ISubredditRepository
    {
        Task<SubredditModel> Create(SubredditModel subreddit);
        Task<SubredditModel> GetByName(string name);
    }
}