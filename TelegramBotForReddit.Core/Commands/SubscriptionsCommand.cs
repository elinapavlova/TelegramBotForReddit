using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class SubscriptionsCommand : BaseCommand
    {
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IUserService _userService;
        
        public SubscriptionsCommand
        (
            string commandName, 
            IUserSubscribeService userSubscribeService, 
            IUserService userService
        ) : base(commandName)
        {
            Name = commandName;
            _userSubscribeService = userSubscribeService;
            _userService = userService;
        }

        public sealed override string Name { get; init; }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            if (message.Text.Split(' ').Length > 1)
                return await client.SendTextMessageAsync(message.Chat.Id, "Необходимо указать команду в виде /subscriptions");

            var userId = message.From.Id;
            
            var isActual = await IsUserActual(userId);
            if (isActual is null or false)
                return await client.SendTextMessageAsync(message.Chat.Id, "Необходимо перезапустить бот с помощью команды /start");

            var userSubscriptions = await GetUserSubscriptions(userId);
            var text = userSubscriptions.Count == 0 
                ? "У вас пока нет подписок." 
                : CreateMessageSubscriptions(userSubscriptions);
            
            return await client.SendTextMessageAsync(message.Chat.Id, text);
        }

        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);
        
        private async Task<List<UserSubscribeDto>> GetUserSubscriptions(long userId)
            => await _userSubscribeService.GetByUserId(userId);
        
        private static string CreateMessageSubscriptions(IEnumerable<UserSubscribeDto> userSubscriptions)
            => userSubscriptions.Aggregate("Вы подписаны на\r\n", (current, subscription) 
                => current + subscription.SubredditName + "\r\n");
    }
}