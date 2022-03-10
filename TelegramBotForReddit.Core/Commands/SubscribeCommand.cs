using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.Reddit;
using TelegramBotForReddit.Core.Services.Subreddit;
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
        private readonly ISubredditService _subredditService;
        private readonly ILogger<SubscribeCommand> _logger;
        public override string Name { get; init; }
        
        public SubscribeCommand
        (
            string commandName, 
            IRedditService redditService, 
            IUserService userService, 
            IUserSubscribeService userSubscribeService,
            ISubredditService subredditService,
            ILogger<SubscribeCommand> logger
        ) 
            : base(commandName)
        {
            _redditService = redditService;
            _userService = userService;
            _userSubscribeService = userSubscribeService;
            _subredditService = subredditService;
            _logger = logger;
        }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
            => await client.SendTextMessageAsync (message.Chat.Id, await CreateMessage(message));

        private async Task<string> CreateMessage(Message message)
        {
            if (message.Text.Split(' ').Length != 2)
                return "Необходимо указать команду в виде /subscribe RedditName";
           
            var subredditName = message.Text.Split(' ')[1];
            
            var isSubredditExist = await IsSubredditExist(subredditName);
            if (isSubredditExist is null or false)
                return "Сабреддит не найден.";

            var userId = message.From.Id;
            var userName = message.From.Username;
            
            var isActual = await IsUserActual(userId);
            if (isActual == null)
                return "Необходимо перезапустить бот с помощью команды /start";
            
            

            var userSubscribe = await GetActualUserSubscribe(userId, subredditName);
            if (userSubscribe != null)
                return $"Вы уже подписаны на {subredditName}.";

            await Subscribe(userId, subredditName);
            _logger.LogInformation($"user {userId} [{userName}] subscribed {subredditName}");
            
            return $"Подписка на {subredditName} подтверждена.";
        }

        private async Task<bool?> IsSubredditExist(string subredditName)
            => await _redditService.IsSubredditExist(subredditName);
        
        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);

        private async Task<UserSubscribeDto> GetActualUserSubscribe(long userId, string subredditName)
            => await _userSubscribeService.GetActual(userId, subredditName);

        private async Task Subscribe(long userSubscribeId, string subredditName)
        {
            var subreddit = await _subredditService.GetByName(subredditName) ?? await _subredditService.Create(subredditName);

            await _userSubscribeService.Subscribe(userSubscribeId, subreddit.Name);
        }
    }
}