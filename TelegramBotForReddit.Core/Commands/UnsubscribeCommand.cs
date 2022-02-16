﻿using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands
{
    public class UnsubscribeCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IMapper _mapper;
        private readonly ILogger<UnsubscribeCommand> _logger;
        
        public UnsubscribeCommand
        (
            string commandName,
            IUserService userService, 
            IUserSubscribeService userSubscribeService,
            IMapper mapper,
            ILogger<UnsubscribeCommand> logger
        ) 
            : base(commandName)
        {
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
                    "Необходимо указать команду в виде /unsubscribe RedditName");
           
            var subredditName = message.Text.Split(' ')[1];

            var user = await _userService.Get(message.From.Id, true);
            if (user == null)
                return await client.SendTextMessageAsync
                    (message.Chat.Id, "Необходимо перезапустить бот с помощью команды /start");

            var userSubscribe = await _userSubscribeService.GetActual(user.Id, subredditName);
            if (userSubscribe == null)
                return await client.SendTextMessageAsync(message.Chat.Id, $"Вы не подписаны на {subredditName}");

            _mapper.Map<UserSubscribeDto>(await _userSubscribeService.Unsubscribe(userSubscribe.Id));
            _logger.LogInformation($"user {message.From.Id} unsubscribed {subredditName}");
            return await client.SendTextMessageAsync (message.Chat.Id, $"Подписка на {subredditName} отменена"); 
        }
    }
}