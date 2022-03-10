using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;

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
            ISubredditService subredditService,
            ILogger<StartCommand> startCommandLogger,
            ILogger<SubscribeCommand> subscribeCommandLogger,
            ILogger<UnsubscribeCommand> unsubscribeCommandLogger,
            ILogger<MakeAdminCommand> makeAdminLogger,
            ILogger<CancelAdminCommand> cancelAdminLogger
        )
        {
            var commands = commandsOptions.Value.Commands;
            
            CommandList = new List<BaseCommand>
            {
                new StartCommand(commands[nameof(StartCommand)], commandsOptions.Value, userService, mapper, startCommandLogger),
                
                new UsingCommand(commands[nameof(UsingCommand)]),
               
                new SubscribeCommand(commands[nameof(SubscribeCommand)], redditService, userService, userSubscribeService, subredditService, subscribeCommandLogger),
                
                new SubredditsCommand(commands[nameof(SubredditsCommand)], redditService),
                
                new UnsubscribeCommand(commands[nameof(UnsubscribeCommand)], userService, userSubscribeService, subredditService, unsubscribeCommandLogger),
                
                new SubscriptionsCommand(commands[nameof(SubscriptionsCommand)], userSubscribeService, userService),
                
                new GetStatisticsCommand(commands[nameof(GetStatisticsCommand)], administratorService, userSubscribeService, userService),
                
                new MakeAdminCommand(commands[nameof(MakeAdminCommand)], administratorService, userService, makeAdminLogger),
                
                new CancelAdminCommand(commands[nameof(CancelAdminCommand)], cancelAdminLogger, administratorService, userService)
            };
        }

        public readonly List<BaseCommand> CommandList;
    }
}