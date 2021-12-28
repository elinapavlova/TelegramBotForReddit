using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Services.Reddit;
using Message = Telegram.Bot.Types.Message;
using Subreddit = Reddit.Controllers.Subreddit;

namespace TelegramBotForReddit.Core.Commands
{
    public class SubredditsCommand : BaseCommand
    {
        private readonly IRedditService _redditService;
        public SubredditsCommand(string commandName, IRedditService redditService) 
            : base(commandName)
        {
            Name = commandName;
            _redditService = redditService;
        }

        public sealed override string Name { get; init; }
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            var subs = _redditService.GetSubreddits("popular");
            return await client.SendTextMessageAsync (message.Chat.Id, CreateMessage(subs));
        }

        private static string CreateMessage(IEnumerable<Subreddit> subreddits)
        {
            return subreddits
                .Aggregate("Список сабреддитов, доступных для подписки:\r\n", 
                    (current, sub) => current + sub.Name + "\r\n");
        }
    }
}