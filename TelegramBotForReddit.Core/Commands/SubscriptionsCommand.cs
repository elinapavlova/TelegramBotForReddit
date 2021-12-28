using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

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
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Необходимо указать команду в виде /subscriptions");

            var user = await _userService.GetById(message.From.Id);
            if (user == null)
                return await client.SendTextMessageAsync
                    (message.Chat.Id, "Необходимо перезапустить бот с помощью команды /start");

            var userSubscribes = await _userSubscribeService.GetByUserId(user.Id);
            if (userSubscribes.Count == 0)
                return await client.SendTextMessageAsync(message.Chat.Id, "У вас пока нет подписок");

            var content = CreateMessage(userSubscribes);
            return await client.SendTextMessageAsync (message.Chat.Id, content); 
        }

        private static string CreateMessage(IEnumerable<UserSubscribeDto> userSubscribes)
        {
            var content = userSubscribes
                .Aggregate("Вы подписаны на\r\n", (current, subscription) 
                    => current + subscription.SubredditName + "\r\n");

            return content;
        }
    }
}