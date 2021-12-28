using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Services.Telegram
{
    public class TelegramService : ITelegramService
    {
        private static List<BaseCommand> _commands;
        private readonly string _botToken;
        private static TelegramBotClient _bot;

        public TelegramService
        (
            AppOptions options,
            CommandsOptions commandsOptions,
            IRedditService redditService,
            IUserService userService,
            IMapper mapper,
            IUserSubscribeService userSubscribeService
        )
        {
            _botToken = options.BotToken;
            _commands = new CommandsStruct(commandsOptions, redditService, userService, mapper, userSubscribeService)
                .CommandList;
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
            return Task.CompletedTask;
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var handler = update.Type switch
                {
                    UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                    _                  => botClient.SendTextMessageAsync(update.Message.Chat.Id, "Команда не обнаружена", 
                                                cancellationToken: cancellationToken)
                };
                
                await handler;
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
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Команда не обнаружена");

                var command = _commands.First(c => c.Name == messageCommand);
                await command.Execute(message, botClient as TelegramBotClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}