using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.Subreddit;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface ISubredditService
    {
        Task<List<SubredditDto>> GetAll();
        Task<SubredditDto> GetByName(string name);
        Task<SubredditDto> Create(string name);
    }
}