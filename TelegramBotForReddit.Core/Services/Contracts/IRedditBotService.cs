using System.Threading.Tasks;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface IRedditBotService
    {
        Task Work();
    }
}