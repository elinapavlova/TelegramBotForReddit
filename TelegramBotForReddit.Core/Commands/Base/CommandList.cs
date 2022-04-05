using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Options;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands.Base
{
    public class CommandList
    {
        public CommandList
        (
            IOptions<CommandsOptions> commandsOptions,
            IRedditService redditService,
            IUserService userService,
            TelegramHttpClient telegramHttpClient,
            IMapper mapper,
            IUserSubscribeService userSubscribeService,
            IAdministratorService administratorService,
            ISubredditService subredditService
        )
        {
            var commands = commandsOptions.Value.Commands;
            
            Commands = CreateCommandList(commandsOptions, redditService, userService, telegramHttpClient, mapper, 
                userSubscribeService, administratorService, subredditService, commands);
        }

        private static List<BaseCommand> CreateCommandList(
            IOptions<CommandsOptions> commandsOptions, 
            IRedditService redditService, 
            IUserService userService, 
            TelegramHttpClient telegramHttpClient, 
            IMapper mapper, 
            IUserSubscribeService userSubscribeService, 
            IAdministratorService administratorService, 
            ISubredditService subredditService, 
            IReadOnlyDictionary<string, string> commands)
        {
            return new List<BaseCommand>
            {
                new StartCommand(commands[nameof(StartCommand)], commandsOptions.Value, userService, telegramHttpClient, mapper),
                
                new UsingCommand(commands[nameof(UsingCommand)], telegramHttpClient),
               
                new SubscribeCommand(commands[nameof(SubscribeCommand)], redditService, userService, userSubscribeService, subredditService, telegramHttpClient),
                
                new SubredditsCommand(commands[nameof(SubredditsCommand)], redditService, telegramHttpClient),
                
                new UnsubscribeCommand(commands[nameof(UnsubscribeCommand)], userService, userSubscribeService, subredditService, telegramHttpClient),
                
                new SubscriptionsCommand(commands[nameof(SubscriptionsCommand)], userSubscribeService, userService, telegramHttpClient),
                
                new GetStatisticsCommand(commands[nameof(GetStatisticsCommand)], administratorService, userSubscribeService, userService, telegramHttpClient),
                
                new MakeAdminCommand(commands[nameof(MakeAdminCommand)], administratorService, userService, telegramHttpClient),
                
                new CancelAdminCommand(commands[nameof(CancelAdminCommand)], administratorService, userService, telegramHttpClient)
            };
        }

        public readonly List<BaseCommand> Commands;
    }
}