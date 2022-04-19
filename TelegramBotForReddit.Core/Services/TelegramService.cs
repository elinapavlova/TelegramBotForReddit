using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models.RedditMedia;

namespace TelegramBotForReddit.Core.Services
{
    public class TelegramService : ITelegramService
    {
        private static List<BaseCommand> _commands;
        private readonly string _redditBaseAddress;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IUserService _userService;
        private readonly IAdministratorService _administratorService;
        private readonly TelegramHttpClient _telegramHttpClient;
        
        public TelegramService
        (
            IOptions<RedditOptions> options,
            CommandList commands,
            IUserSubscribeService userSubscribeService,
            IUserService userService,
            IAdministratorService administratorService,
            TelegramHttpClient telegramHttpClient
        )
        {
            _commands = commands.Commands;
            _userSubscribeService = userSubscribeService;
            _redditBaseAddress = options.Value.BaseAddress;
            _userService = userService;
            _administratorService = administratorService;
            _telegramHttpClient = telegramHttpClient;
        }
        
        public DefaultUpdateHandler CreateDefaultUpdateHandler()
            => new (HandleUpdateAsync, HandleErrorAsync);

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message :
                        await BotOnMessageReceived(update.Message);
                        break;
                    
                    case UpdateType.MyChatMember :
                        if (update.MyChatMember.NewChatMember.Status != ChatMemberStatus.Kicked) 
                            break;
                        await StopBot(update.MyChatMember.From.Id);
                        break;
                }
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }
        
        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => "Telegram API Error:\n" +
                                                           $"[{apiRequestException.ErrorCode}]\n" +
                                                           $"{apiRequestException.Message}",
                _                                       => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            Logger.Logger.LogError($"Telegram API handling updates Error: {errorMessage}");
            return Task.CompletedTask;
        }
        
        private async Task BotOnMessageReceived(Message message)
        {
            // Если сообщение не текст - игнорировать
            if (message.Type != MessageType.Text)
                return;
            try
            {
                var command = GetCommandByName(message);
                if (command is null)
                {
                    await _telegramHttpClient.SendTextMessage(message.Chat.Id, "Команда не обнаружена");
                    return;
                }
                
                await command.Execute(message);
                await SendReplyKeyboard(message);
            }
            catch (Exception e)
            {
                Logger.Logger.LogError($"Telegram API receiving message Error: {e.Message}\r\n" +
                                 $"user : {message.From.Id} [ {message.From.Username} ]\r\n" +
                                 $"message : {message.Text}");
            }
        }

        private static BaseCommand GetCommandByName(Message message)
        {
            // Отделение первого слова (команды)
            var commandName = message.Text.Split(' ').FirstOrDefault();
            var command = _commands.FirstOrDefault(command => command.Contains(commandName));
            return command;
        }

        private async Task SendReplyKeyboard(Message message)
        {
            var keyboard = GetReplyKeyboard();
            await _telegramHttpClient.SendTextMessage(message.Chat.Id, "Выбрать команду:", keyboard);
        }

        private async Task StopBot(long userId)
        {
            var isUserAdmin = await _administratorService.IsUserAdmin(userId);
            if (isUserAdmin)
                await _administratorService.Delete(userId);
            
            await _userSubscribeService.UnsubscribeAll(userId);
            await _userService.StopBot(userId);
            
            Logger.Logger.LogInfo($"user {userId} stopped bot");
        }

        public async Task SendMessage(Media media, UserSubscribeDto user, Post post)
        {
            var keyboard = GetInlineKeyboard(CreatePostUrl(post), media);
            try
            {
                if (media != null)
                    await _telegramHttpClient.SendPhotoMessage(user.UserId, $"{media.Images.First().Source.Url}", post, keyboard);
                else
                    await _telegramHttpClient.SendTextMessage(user.UserId, $"{post.Subreddit}\r\n{post.Title}\r\n", keyboard);
            }
            catch(ApiRequestException ex)
            {
                await _telegramHttpClient.SendTextMessage(user.UserId, CreateUnsuccessfulContentMessage(post), keyboard);
                
                Logger.Logger.LogError($"Telegram API upload content Error: {ex.ErrorCode}. {ex.Message} content: {media?.Url}");
            }
        }

        private static string CreateUnsuccessfulContentMessage(Post post)
        {
            return $"[Не удалось загрузить контент]\r\n{post.Subreddit}\r\n{post.Title}\r\n";
        }

        private static ReplyKeyboardMarkup GetReplyKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var columns = new List<KeyboardButton>();
            var lastIndex = _commands.Count - 1;

            // Алгоритм для размещения команд в виде кнопок reply-клавиатуры
            foreach (var command in _commands)
            {
                var index = _commands.IndexOf(command);
                columns.Add(command.Name);
                
                if (index % 2 == 0 && index != lastIndex) 
                    continue;
                
                rows.Add(columns.ToArray());
                columns = new List<KeyboardButton>();
            }

            keyboard.Keyboard = rows.ToArray();
            return keyboard;
        }

        private static InlineKeyboardMarkup GetInlineKeyboard(string postUrl, Media media)
        {
            // Если нет контента или контент - картинка
            return media == null || media.Enabled 
                ? new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] {InlineKeyboardButton.WithUrl("Перейти к посту", postUrl)}
                    })
                // Если контент - видео, ссылка, стрим и другое
                : new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] {InlineKeyboardButton.WithUrl("Перейти к посту", postUrl)},
                        new[] {InlineKeyboardButton.WithUrl("Перейти к ресурсу", media.Url)}
                    });
        }
        
        private string CreatePostUrl(Post post)
        {
            return $"{_redditBaseAddress}{post.Permalink}";
        }
    }
}