using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands.Base
{
    public class Commands
    {
        public Commands
        (
            IOptions<CommandsOptions> commandsOptions,
            IRedditService redditService,
            IUserService userService,
            IMapper mapper,
            IUserSubscribeService userSubscribeService,
            IAdministratorService administratorService,
            ILogger<StartCommand> startCommandLogger,
            ILogger<SubscribeCommand> subscribeCommandLogger,
            ILogger<UnsubscribeCommand> unsubscribeCommandLogger
        )
        {
            var commands = commandsOptions.Value.Commands;
            
            CommandList = new List<BaseCommand>
            {
                new StartCommand(commands[nameof(StartCommand)], commandsOptions.Value, userService, mapper, startCommandLogger),
                new UsingCommand(commands[nameof(UsingCommand)]),
                new SubscribeCommand(commands[nameof(SubscribeCommand)], redditService, userService, userSubscribeService, 
                    mapper, subscribeCommandLogger),
                new SubredditsCommand(commands[nameof(SubredditsCommand)], redditService),
                new UnsubscribeCommand(commands[nameof(UnsubscribeCommand)], userService, userSubscribeService, mapper,
                    unsubscribeCommandLogger),
                new SubscriptionsCommand(commands[nameof(SubscriptionsCommand)], userSubscribeService, userService),
                new GetStatisticsCommand(commands[nameof(GetStatisticsCommand)], administratorService, userSubscribeService)
            };
        }

        public readonly List<BaseCommand> CommandList;
    }
}