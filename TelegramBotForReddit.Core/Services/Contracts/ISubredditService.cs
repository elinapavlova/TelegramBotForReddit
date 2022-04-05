using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.Subreddit;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface ISubredditService
    {
        Task<SubredditDto> GetByName(string name);
        Task<SubredditDto> Create(string name);
    }
}