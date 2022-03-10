using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.Contracts
{
    public interface ISubredditRepository
    {
        Task<List<SubredditModel>> GetAll();
        Task<SubredditModel> Create(SubredditModel subreddit);
        Task<SubredditModel> GetByName(string name);
    }
}