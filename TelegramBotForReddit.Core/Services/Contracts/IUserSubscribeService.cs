using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.Subreddit;
using TelegramBotForReddit.Core.Dto.UserSubscribe;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface IUserSubscribeService
    {
        Task<UserSubscribeDto> Subscribe(long userId, string subredditName);
        Task<UserSubscribeDto> Unsubscribe(Guid id);
        Task UnsubscribeAll(long userId);
        Task<UserSubscribeDto> GetActual(long userId, string subredditName);
        Task<List<UserSubscribeDto>> GetByUserId(long userId);
        Task<List<UserSubscribeDto>> GetBySubredditName(string name);
        Task<List<SubredditStatisticsDto>> GetPopularestSubreddits();
        Task<int?> GetAverageNumberOfSubscribes();
    }
}