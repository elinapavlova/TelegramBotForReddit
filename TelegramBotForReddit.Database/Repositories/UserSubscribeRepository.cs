using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Database.Repositories
{
    public class UserSubscribeRepository : IUserSubscribeRepository
    {
        private readonly AppDbContext _context;
        
        // Блокировка получения записей
        private readonly Mutex _mutex = new ();

        public UserSubscribeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserSubscribeModel> Create(UserSubscribeModel userSubscribe)
        {
            await _context.Set<UserSubscribeModel>().AddAsync(userSubscribe);
            await _context.SaveChangesAsync();
            return userSubscribe;
        }

        public async Task<UserSubscribeModel> Update(UserSubscribeModel userSubscribe)
        {
            _context.Update(userSubscribe);
            await _context.SaveChangesAsync();
            return userSubscribe;
        }

        public async Task<UserSubscribeModel> Get(long userId, string subredditName, bool isActual)
        {
            return isActual
                ? await _context.UserSubscribes.FirstOrDefaultAsync(us =>
                    us.UserId == userId
                    && us.SubredditName == subredditName
                    && us.DateUnsubscribed == null)
                : await _context.UserSubscribes.FirstOrDefaultAsync(us =>
                    us.UserId == userId
                    && us.SubredditName == subredditName
                    && us.DateUnsubscribed != null);
        }

        public async Task<List<UserSubscribeModel>> GetByUserId(long userId)
            => await _context.UserSubscribes
                .Where(u => u.UserId == userId && u.DateUnsubscribed == null)
                .ToListAsync();

        public async Task<UserSubscribeModel> GetById(Guid id)
            => await _context.UserSubscribes.FirstOrDefaultAsync(us => us.Id == id);

        public async Task<List<UserSubscribeModel>> GetBySubredditName(string name)
        {
            _mutex.WaitOne();
            
            var result = await _context.UserSubscribes
                .Where(us => us.SubredditName == name && us.DateUnsubscribed == null)
                .ToListAsync();
            
            _mutex.ReleaseMutex();
            
            return result;
        }

        public async Task<List<SubredditStatisticsModel>> GetPopularestSubreddits()
        {
            var subreddits = await _context.UserSubscribes
                .Where(us => us.DateUnsubscribed == null)
                .GroupBy(us => us.SubredditName)
                .Select(us => new SubredditStatisticsModel {Name = us.Key, CountSubscribes = us.Count()})
                .OrderByDescending(us => us.CountSubscribes)
                .ToListAsync();

            var popular = subreddits.FindAll(us => us.CountSubscribes == subreddits.Max(_ => _.CountSubscribes));
            return popular;
        }

        public async Task<int?> GetAverageNumberOfSubscribes()
        {
            var number = await _context.UserSubscribes
                .Where(us => us.DateUnsubscribed == null)
                .GroupBy(us => us.UserId)
                .Select(us => us.Count())
                .AverageAsync(us => us);

            var numberRound = Math.Round(number).ToString(CultureInfo.InvariantCulture);
            int.TryParse(numberRound, out var result);
            return result;
        }

        public async Task<List<WordCloudModel>> GetSubscribesNameCount()
        {
            var result = await _context.UserSubscribes.GroupBy(us => us.SubredditName)
                .Select(us => new WordCloudModel
                {
                    Name = us.Key,
                    Count = us.Count()
                })
                .ToListAsync();
            return result;
        }
        
        // Названия сабреддитов, на которые есть актуальные подписки
        public async Task<List<string>> GetSubredditNames()
            => await _context.UserSubscribes.Where(us => us.DateUnsubscribed == null)
                .GroupBy(us => us.SubredditName)
                .Select(us => us.Key)
                .ToListAsync();
    }
}