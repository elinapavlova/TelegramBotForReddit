using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Database.Repositories
{
    public class SubredditRepository : ISubredditRepository
    {
        private readonly AppDbContext _context;

        public SubredditRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubredditModel>> GetAll()
            => await _context.Subreddits.ToListAsync();
        
        public async Task<List<SubredditModel>> GetAllActual()
        {
            var subredditsName = await _context.Subreddits
                .Select(s => new {s.Name})
                .ToListAsync();

            var actualSubreddits = new List<SubredditModel>();
            
            foreach (var item in subredditsName)
            {
                var isActual = _context.UserSubscribes
                    .Where(us => us.SubredditName == item.Name)
                    .Any(us => us.DateUnsubscribed == null);

                if (!isActual)
                    continue;

                var subreddit = await GetByName(item.Name);
                if(subreddit is not null)
                    actualSubreddits.Add(subreddit);
            }

            return actualSubreddits;
        }
        
        public async Task<SubredditModel> Create(SubredditModel subreddit)
        {
            await _context.Subreddits.AddAsync(subreddit);
            await _context.SaveChangesAsync();
            
            return subreddit;
        }
        
        public async Task<SubredditModel> GetByName(string name)
            => await _context.Subreddits
                .Where(s => s.Name == name)
                .FirstOrDefaultAsync();
    }
}