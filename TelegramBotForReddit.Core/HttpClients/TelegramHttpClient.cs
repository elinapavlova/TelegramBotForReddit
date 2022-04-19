using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForReddit.Core.Options;

namespace TelegramBotForReddit.Core.HttpClients
{
    public class TelegramHttpClient
    {
        private readonly string _botToken;
        private TelegramBotClient _bot;

        public TelegramHttpClient(IOptions<AppOptions> options)
        {
            _botToken = options.Value.BotToken;
        }
        
        public TelegramBotClient CreateBot()
        {
            _bot = new TelegramBotClient(_botToken);
            return _bot;
        }

        public async Task SendTextMessage(long chatId, string text, ReplyKeyboardMarkup keyboard, ParseMode parseMode = ParseMode.Default)
        {
            if (parseMode == ParseMode.Default)
            {
                await _bot.SendTextMessageAsync(chatId, text, replyMarkup: keyboard);
                return;
            }
            
            await _bot.SendTextMessageAsync(chatId, text, parseMode, replyMarkup: keyboard);
        }

        public async Task SendTextMessage(long chatId, string text, InlineKeyboardMarkup keyboard, ParseMode parseMode = ParseMode.Default)
        {
            if (parseMode == ParseMode.Default)
            {
                await _bot.SendTextMessageAsync(chatId, text, replyMarkup: keyboard);
                return;
            }
            
            await _bot.SendTextMessageAsync(chatId, text, parseMode, replyMarkup: keyboard);
        }
        
        public async Task SendTextMessage(long chatId, string text, ParseMode parseMode = ParseMode.Default)
        {
            if (parseMode == ParseMode.Default)
            {
                await _bot.SendTextMessageAsync(chatId, text);
                return;
            }
            
            await _bot.SendTextMessageAsync(chatId, text, parseMode);
        }

        public async Task SendPhotoMessage(long chatId, string url, Post post, InlineKeyboardMarkup keyboard)
        {
            await _bot.SendPhotoAsync(chatId, url, 
                $"{post.Subreddit}\r\n{post.Title}\r\n ", 
                replyMarkup: keyboard);
        }
    }
}