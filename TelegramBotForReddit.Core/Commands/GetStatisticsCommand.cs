﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Core.Services.UserSubscribe;

namespace TelegramBotForReddit.Core.Commands
{
    public class GetStatisticsCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserSubscribeService _userSubscribeService;
        private readonly IUserService _userService;
        
        public GetStatisticsCommand
        (
            string commandName, 
            IAdministratorService administratorService,
            IUserSubscribeService userSubscribeService,
            IUserService userService
        ) 
            : base(commandName)
        {
            Name = commandName;
            _administratorService = administratorService;
            _userSubscribeService = userSubscribeService;
            _userService = userService;
        }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
        {
            var isUserAdmin = await _administratorService.IsUserAdmin(message.From.Id);
            if (!isUserAdmin)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Команда доступна только администраторам.");
            
            var text = await GetStatistics();
            return await client.SendTextMessageAsync(message.Chat.Id, text);
        }

        private async Task<string> GetStatistics()
        {
            var message = $"Актуальная статистика на {DateTime.Now}\r\n";
            message += await GetStatisticsByUsingsBot();
            message += "\r\n\r\nСамый(ые) популярный(ые) сабреддит(ы) " + await GetPopularestSubreddits();
            message += "\r\n\r\nСреднее количество подписок -- " + await GetAverageNumberOfSubscribes();

            return message;
        }

        private async Task<string> GetStatisticsByUsingsBot()
        {
            var today = DateTime.Today.ToShortDateString();
            var dateWeekAgo = DateTime.Today.AddDays(-6);
            var dateMonthAgo = DateTime.Today.AddMonths(-1);
            var message = string.Empty;

            message += $"\r\nИспользуют -- {await GetCountOfUsers()} чел.";
            
            message += "\r\n\r\nОстановили бот" +
                       $"\r\n-- за неделю (с {dateWeekAgo.ToShortDateString()} до {today}) -- " +
                       await GetCountOfStopsBotByDate(dateWeekAgo) + " чел." +
                       $"\r\n-- за месяц (с {dateMonthAgo.ToShortDateString()} до {today}) -- " + 
                       await GetCountOfStopsBotByDate(dateMonthAgo) + " чел.";
            
            message += "\r\n\r\n(Пере)запустили бот" +
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
        private async Task<int> GetAverageNumberOfSubscribes()
            => await _userSubscribeService.GetAverageNumberOfSubscribes();
    }
}