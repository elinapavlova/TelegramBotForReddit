using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.Contracts
{
    public interface IUserSubscribeRepository
    {
        Task<UserSubscribeModel> Create(UserSubscribeModel userSubscribe);
        Task<UserSubscribeModel> Update(UserSubscribeModel userSubscribe);
        Task<UserSubscribeModel> Get(long userId, string subredditName, bool isActual);
        Task<List<UserSubscribeModel>> GetByUserId(long userId);
        Task<UserSubscribeModel> GetById(Guid id);
        Task<List<UserSubscribeModel>> GetBySubredditName(string name);
        Task<List<SubredditStatisticsModel>> GetPopularestSubreddits();
        Task<int?> GetAverageNumberOfSubscribes();
        Task<List<WordCloudModel>> GetSubscribesNameCount();
        Task<List<string>> GetSubredditNames();
    }
}