﻿using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;
using Message = Telegram.Bot.Types.Message;

namespace TelegramBotForReddit.Core.Commands
{
    public class SubscribeCommand : BaseCommand
    {
        private readonly IRedditService _redditService;
        private readonly IUserService _userService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscribeCommand> _logger;
        
        public SubscribeCommand
        (
            string commandName, 
            IRedditService redditService, 
            IUserService userService, 
            IUserSubscribeService userSubscribeService,
            IMapper mapper,
            ILogger<SubscribeCommand> logger
        ) 
            : base(commandName)
        {
            _redditService = redditService;
            _userService = userService;
            _userSubscribeService = userSubscribeService;
            _mapper = mapper;
            _logger = logger;
        }

        public override string Name { get; init; }
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            if (message.Text.Split(' ').Length != 2)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Необходимо указать команду в виде /subscribe RedditName");
           
            var subredditName = message.Text.Split(' ')[1];

            var subs = _redditService.GetSubreddits("popular");
            if (!subs.Exists(sub => sub.Name == subredditName))
                return await client.SendTextMessageAsync (message.Chat.Id, "Сабреддит с таким названием не найден");

            var userId = message.From.Id;
            var isActual = await _userService.IsActual(userId);
            if (isActual == null)
                return await client.SendTextMessageAsync
                    (message.Chat.Id, "Необходимо перезапустить бот с помощью команды /start");

            var userSubscribe = await _userSubscribeService.GetActual(userId, subredditName);
            if (userSubscribe != null)
                return await client.SendTextMessageAsync(message.Chat.Id, $"Вы уже подписаны на {subredditName}");

            _mapper.Map<UserSubscribeDto>(await _userSubscribeService.Subscribe(userId, subredditName));
            _logger.LogInformation($"user {userId} subscribed {subredditName}");
            return await client.SendTextMessageAsync (message.Chat.Id, $"Подписка на {subredditName} подтверждена"); 
        }
    }
}