using System;
using System.Threading;
using System.Threading.Tasks;
using Reddit.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Database.Models.RedditMedia;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface ITelegramService
    {
        TelegramBotClient CreateBot();
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task SendMessage(Media media, UserSubscribeDto user, Post post);
    }
}