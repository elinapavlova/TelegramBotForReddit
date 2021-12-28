using System.Collections.Generic;
using AutoMapper;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands.Base
{
    public readonly struct CommandsStruct
    {
        public CommandsStruct
        (
            CommandsOptions commandsOptions,
            IRedditService redditService,
            IUserService userService,
            IMapper mapper,
            IUserSubscribeService userSubscribeService
        )
        {
            var commands = commandsOptions.Commands;
            
            CommandList = new List<BaseCommand>
            {
                new StartCommand(commands[nameof(StartCommand)], commandsOptions, userService, mapper),
                new UsingCommand(commands[nameof(UsingCommand)]),
                new SubscribeCommand(commands[nameof(SubscribeCommand)], redditService, userService, 
                    userSubscribeService, mapper),
                new SubredditsCommand(commands[nameof(SubredditsCommand)], redditService),
                new UnsubscribeCommand(commands[nameof(UnsubscribeCommand)], userService, userSubscribeService, mapper),
                new SubscriptionsCommand(commands[nameof(SubscriptionsCommand)], userSubscribeService, userService)
            };
        }

        public readonly List<BaseCommand> CommandList;
    }
}