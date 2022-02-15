using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands
{
    public class GetStatisticsCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserSubscribeService _userSubscribeService;

        public GetStatisticsCommand
        (
            string commandName, 
            IAdministratorService administratorService,
            IUserSubscribeService userSubscribeService
        ) 
            : base(commandName)
        {
            Name = commandName;
            _administratorService = administratorService;
            _userSubscribeService = userSubscribeService;
        }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            var isUserAdmin = await IsUserAdmin(message.From.Id);
            if (!isUserAdmin)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Команда доступна только администраторам.");
            
            var text = await GetStatistics();
            return await client.SendTextMessageAsync(message.Chat.Id, text);
        }

        private async Task<bool> IsUserAdmin(long id)
        {
            var admin = await _administratorService.GetByUserId(id);
            return admin != null;
        }

        private async Task<string> GetStatistics()
        {
            var message = $"Актуальная статистика на {DateTime.Now}\r\n";
            message += "\r\n\r\nСамый(ые) популярный(ые) сабреддит(ы) " + await GetPopularestSubreddits();
            message += "\r\n\r\nСреднее количество подписок -- " + await GetAverageNumberOfSubscribes();

            return message;
        }

        private async Task<string> GetPopularestSubreddits()
        {
            var message = string.Empty;
            var subreddits = await _userSubscribeService.GetPopularestSubreddits();

            return subreddits.Aggregate(message, (current, subreddit) 
                => current + $"\r\n-- {subreddit.Name} -- {subreddit.CountSubscribes}");
        }
        private async Task<int> GetAverageNumberOfSubscribes()
            => await _userSubscribeService.GetAverageNumberOfSubscribes();
    }
}