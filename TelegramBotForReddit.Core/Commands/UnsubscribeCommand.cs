using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class UnsubscribeCommand : BaseCommand
    {
        private readonly IUserService _userService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly ISubredditService _subredditService;
        private readonly TelegramHttpClient _telegramHttpClient;
        public override string Name { get; init; }
        
        public UnsubscribeCommand
        (
            string commandName,
            IUserService userService, 
            IUserSubscribeService userSubscribeService,
            ISubredditService subredditService,
            TelegramHttpClient telegramHttpClient
        ) 
            : base(commandName)
        {
            _userService = userService;
            _userSubscribeService = userSubscribeService;
            _subredditService = subredditService;
            _telegramHttpClient = telegramHttpClient;
        }

        public override async Task Execute(Message message)
            => await _telegramHttpClient.SendTextMessage(message.Chat.Id, await CreateMessage(message)); 

        private async Task<string> CreateMessage(Message message)
        {
            if (message.Text.Split(' ').Length != 2)
                return "Необходимо указать команду в виде /unsubscribe RedditName";
           
            var subredditName = message.Text.Split(' ')[1];
            var userId = message.From.Id;
            var userName = message.From.Username;
            
            var isActual = await IsUserActual(userId);
            if (isActual == null)
                return "Необходимо перезапустить бот с помощью команды /start";

            var userSubscribe = await GetActualUserSubscribe(userId, subredditName);
            if (userSubscribe == null)
                return $"Вы не подписаны на {subredditName}.";

            await Unsubscribe(userSubscribe.Id);
            Logger.Logger.LogInfo($"user {userId} [{userName}] unsubscribed {subredditName}");
            
            return $"Подписка на {subredditName} отменена."; 
        }
        
        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);

        private async Task<UserSubscribeDto> GetActualUserSubscribe(long userId, string subredditName)
        {
            var subreddit = await _subredditService.GetByName(subredditName);
            if (subreddit == null)
                return null;
            
            return await _userSubscribeService.GetActual(userId, subredditName);
        }
        
        private async Task Unsubscribe(Guid userSubscribeId)
            => await _userSubscribeService.Unsubscribe(userSubscribeId);
    }
}