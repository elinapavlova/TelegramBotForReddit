using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Commands.Base;
using Message = Telegram.Bot.Types.Message;
using Reddit.Controllers;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class SubredditsCommand : BaseCommand
    {
        private readonly IRedditService _redditService;
        private readonly TelegramHttpClient _telegramHttpClient;
        public sealed override string Name { get; init; }
        
        
        public SubredditsCommand
        (
            string commandName, 
            IRedditService redditService,
            TelegramHttpClient telegramHttpClient
        ) 
            : base(commandName)
        {
            Name = commandName;
            _redditService = redditService;
            _telegramHttpClient = telegramHttpClient;
        }

        public override async Task Execute(Message message)
        { 
            var subreddits = GetSubreddits("popular");
            await _telegramHttpClient.SendTextMessage(message.Chat.Id, CreateMessageSubreddits(subreddits));
        }

        private IEnumerable<Subreddit> GetSubreddits(string category)
            => _redditService.GetSubreddits(category);
        
        private static string CreateMessageSubreddits(IEnumerable<Subreddit> subreddits)
            => subreddits.Aggregate($"Список самых популярных сабреддитов на {DateTime.Now.ToShortDateString()}:\r\n", 
                    (current, sub) => current + sub.Name + "\r\n");
    }
}