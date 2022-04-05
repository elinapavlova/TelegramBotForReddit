using System.Threading.Tasks;
using Reddit.Controllers;
using Telegram.Bot.Extensions.Polling;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Database.Models.RedditMedia;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface ITelegramService
    {
        Task SendMessage(Media media, UserSubscribeDto user, Post post);
        DefaultUpdateHandler CreateDefaultUpdateHandler();
    }
}