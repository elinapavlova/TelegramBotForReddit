using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Services.Reddit;
using Message = Telegram.Bot.Types.Message;
using Reddit.Controllers;

namespace TelegramBotForReddit.Core.Commands
{
    public class SubredditsCommand : BaseCommand
    {
        private readonly IRedditService _redditService;
        public sealed override string Name { get; init; }
        
        public SubredditsCommand
        (
            string commandName, 
            IRedditService redditService
        ) 
            : base(commandName)
        {
            Name = commandName;
            _redditService = redditService;
        }

        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        { 
            var subreddits = GetSubreddits("popular");
            return await client.SendTextMessageAsync(message.Chat.Id, CreateMessageSubreddits(subreddits));
        }

        private IEnumerable<Subreddit> GetSubreddits(string category)
            => _redditService.GetSubreddits(category);
        
        private static string CreateMessageSubreddits(IEnumerable<Subreddit> subreddits)
            => subreddits.Aggregate($"Список самых популярных сабреддитов на {DateTime.Now.ToShortDateString()}:\r\n", 
                    (current, sub) => current + sub.Name + "\r\n");
    }
}