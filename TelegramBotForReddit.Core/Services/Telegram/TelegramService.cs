﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Controllers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;
using TelegramBotForReddit.Database.Models.RedditMedia;

namespace TelegramBotForReddit.Core.Services.Telegram
{
    public class TelegramService : ITelegramService
    {
        private static List<BaseCommand> _commands;
        private readonly string _botToken;
        private readonly string _redditBaseAddress;
        private static TelegramBotClient _bot;
        private static ILogger<TelegramService> _logger;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IUserService _userService;
        private readonly IAdministratorService _administratorService;
        
        public TelegramService
        (
            IOptions<AppOptions> options,
            ILogger<TelegramService> logger,
            Commands.Base.Commands commands,
            IUserSubscribeService userSubscribeService,
            IUserService userService,
            IAdministratorService administratorService
        )
        {
            _botToken = options.Value.BotToken;
            _commands = commands.CommandList;
            _logger = logger;
            _userSubscribeService = userSubscribeService;
            _redditBaseAddress = options.Value.RedditBaseAddress;
            _userService = userService;
            _administratorService = administratorService;
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
                        await StopBot(update.MyChatMember.From.Id);
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
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрать команду:", replyMarkup: keyboard);

            }
            catch (Exception e)
            {
                _logger.LogError($"Telegram API receiving message Error: {e.Message}");
            }
        }

        private async Task StopBot(long userId)
        {
            var isUserAdmin = await _administratorService.IsUserAdmin(userId);
            if (isUserAdmin)
                await _administratorService.Delete(userId);
            
            await _userSubscribeService.UnsubscribeAll(userId);
            await _userService.StopBot(userId);
            
            _logger.LogInformation($"user {userId} stopped bot");
        }

        public async Task SendMessage(Media media, UserSubscribeDto user, Post post)
        {
            var keyboard = GetInlineKeyboard($"{_redditBaseAddress}{post.Permalink}", media);
            try
            {
                if (media != null)
                    await _bot.SendPhotoAsync(user.UserId,
                        $"{media.Images.First().Source.Url}",
                        $"{post.Subreddit}\r\n{post.Title}\r\n ",
                        replyMarkup: keyboard);
                else
                    await _bot.SendTextMessageAsync(user.UserId,
                        $"{post.Subreddit}\r\n{post.Title}\r\n",
                        replyMarkup: keyboard);
            }
            catch(ApiRequestException ex)
            {
                await _bot.SendTextMessageAsync(user.UserId, 
                    $"[Не удалось загрузить контент]\r\n{post.Subreddit}\r\n{post.Title}\r\n",
                    replyMarkup: keyboard);
                _logger.LogError($"Telegram API upload content Error: {ex.ErrorCode}. {ex.Message} content: {media.Url}");
            }
        }
        
        private static ReplyKeyboardMarkup GetReplyKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var columns = new List<KeyboardButton>();
            var lastIndex = _commands.Count - 1;

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
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Перейти к ресурсу", media.Url)
                        }
                    });
        }
    }
}