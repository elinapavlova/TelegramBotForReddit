using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class GetStatisticsCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IUserService _userService;
        private readonly TelegramHttpClient _telegramHttpClient;
        
        public GetStatisticsCommand
        (
            string commandName, 
            IAdministratorService administratorService,
            IUserSubscribeService userSubscribeService,
            IUserService userService,
            TelegramHttpClient telegramHttpClient
        ) 
            : base(commandName)
        {
            Name = commandName;
            _administratorService = administratorService;
            _userSubscribeService = userSubscribeService;
            _userService = userService;
            _telegramHttpClient = telegramHttpClient;
        }

        public override async Task Execute(Message message)
        {
            var isUserAdmin = await IsUserAdmin(message.From.Id);
            if (!isUserAdmin)
            {
                await _telegramHttpClient.SendTextMessage(message.Chat.Id, "Команда доступна только администраторам\\.");
                return;
            }
            
            var text = $"Актуальная статистика на <b><u>{DateTime.Now}</u></b>\r\n";
            
            text += await GetStatisticsByUsingsBot();
            
            text += "\r\n\r\n<b>Среднее количество подписок</b> -- " + await GetAverageNumberOfSubscribes();
            
            var popularestSubreddits = await GetPopularestSubreddits();
            text += popularestSubreddits.Length > 1 
                ? "\r\n\r\n<b>Самые популярные сабреддиты</b> " + popularestSubreddits
                : "\r\n\r\n<b>Самый популярный сабреддит</b> " + popularestSubreddits;
            
            await _telegramHttpClient.SendTextMessage(message.Chat.Id, text, ParseMode.Html);
        }
        
        private async Task<bool> IsUserAdmin(long userId)
            => await _administratorService.IsUserAdmin(userId);

        private async Task<string> GetStatisticsByUsingsBot()
        {
            var today = DateTime.Today.ToShortDateString();
            var dateWeekAgo = DateTime.Today.AddDays(-6);
            var dateMonthAgo = DateTime.Today.AddMonths(-1);
            var message = string.Empty;

            message += $"\r\n<b>Используют</b> -- {await GetCountOfUsers()} чел.";
            
            message += "\r\n\r\n<b>Остановили бот</b>" +
                       $"\r\n-- за неделю (с {dateWeekAgo.ToShortDateString()} до {today}) -- " +
                       await GetCountOfStopsBotByDate(dateWeekAgo) + " чел." +
                       $"\r\n-- за месяц (с {dateMonthAgo.ToShortDateString()} до {today}) -- " + 
                       await GetCountOfStopsBotByDate(dateMonthAgo) + " чел.";
            
            message += "\r\n\r\n<b>(Пере)запустили бот</b>" +
                       $"\r\n-- за неделю (с {dateWeekAgo.ToShortDateString()} до {today}) -- " +
                       await GetCountOfStartsBotByDate(dateWeekAgo) + " чел." +
                       $"\r\n-- за месяц (с {dateMonthAgo.ToShortDateString()} до {today}) -- " + 
                       await GetCountOfStartsBotByDate(dateMonthAgo) + " чел.";

            return message;
        }

        private async Task<int> GetCountOfUsers()
            => await _userService.Count();
        
        private async Task<int> GetCountOfStopsBotByDate(DateTime date)
            => await _userService.GetCountOfStopsBotByDate(date);
        
        private async Task<int> GetCountOfStartsBotByDate(DateTime date)
            => await _userService.GetCountOfStartsBotByDate(date);

        private async Task<string> GetPopularestSubreddits()
        {
            var message = string.Empty;
            var subreddits = await _userSubscribeService.GetPopularestSubreddits();

            return subreddits.Aggregate(message, (current, subreddit) 
                => current + $"\r\n-- {subreddit.Name} -- {subreddit.CountSubscribes} чел.");
        }

        private async Task<int?> GetAverageNumberOfSubscribes()
            => await _userSubscribeService.GetAverageNumberOfSubscribes();
    }
}