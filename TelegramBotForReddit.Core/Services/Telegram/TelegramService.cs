using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Services.Telegram
{
    public class TelegramService : ITelegramService
    {
        private static List<BaseCommand> _commands;
        private readonly string _botToken;
        private static TelegramBotClient _bot;
        private static ILogger<TelegramService> _logger;
        private readonly IUserSubscribeService _userSubscribeService;
        
        public TelegramService
        (
            IOptions<AppOptions> options,
            ILogger<TelegramService> logger,
            Commands.Base.Commands commands,
            IUserSubscribeService userSubscribeService
        )
        {
            _botToken = options.Value.BotToken;
            _commands = commands.CommandList;
            _logger = logger;
            _userSubscribeService = userSubscribeService;
        }

        public TelegramBotClient CreateBot()
        {
            _bot = new TelegramBotClient(_botToken);
            return _bot;
        }
        
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => "Telegram API Error:\n" +
                                                           $"[{apiRequestException.ErrorCode}]\n" +
                                                           $"{apiRequestException.Message}",
                _                                       => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            _logger.LogError($"Telegram API handling updates Error: {errorMessage}");
            return Task.CompletedTask;
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message :
                        await BotOnMessageReceived(botClient, update.Message);
                        break;
                    
                    case UpdateType.MyChatMember :
                        if (update.MyChatMember.NewChatMember.Status != ChatMemberStatus.Kicked) 
                            break;
                        await _userSubscribeService.UnsubscribeAll(update.MyChatMember.From.Id);
                        _logger.LogInformation($"user {update.MyChatMember.From.Id} stopped bot");
                        break;
                        
                    default :
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда не обнаружена",
                            cancellationToken: cancellationToken);
                        break;
                }
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            // Если сообщение не текст - игнорировать
            if (message.Type != MessageType.Text)
                return;
            try
            {
                // Отделение первого слова (команды)
                var messageCommand = message.Text.Split(' ').FirstOrDefault();
                var isExistingCommand = _commands.Any(comm => comm.Contains(messageCommand));

                if (!isExistingCommand)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Команда не обнаружена");
                    return;
                }

                var command = _commands.First(c => c.Name == messageCommand);
                await command.Execute(message, botClient as TelegramBotClient);

                var keyboard = GetReplyKeyboard();
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите следующее действие", replyMarkup: keyboard);

            }
            catch (Exception e)
            {
                _logger.LogError($"Telegram API receiving message Error: {e.Message}");
            }
        }

        private static ReplyKeyboardMarkup GetReplyKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var columns = new List<KeyboardButton>();

            foreach (var command in _commands)
            {                
                columns.Add(command.Name);
                
                if (_commands.IndexOf(command) % 2 == 0) 
                    continue;
                
                rows.Add(columns.ToArray());
                columns = new List<KeyboardButton>();
            }

            keyboard.Keyboard = rows.ToArray();
            return keyboard;
        }
    }
}