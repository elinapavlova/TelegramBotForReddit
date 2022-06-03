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

        public async Task<SubredditModel> Create(SubredditModel subreddit)
        {
            await _context.Subreddits.AddAsync(subreddit);
            await _context.SaveChangesAsync();
            
            return subreddit;
        }
        
        public async Task<SubredditModel> Remove(SubredditModel subreddit)
        {
            _context.Subreddits.Remove(subreddit);
            await _context.SaveChangesAsync();
            
            return subreddit;
        }
        
        public async Task<SubredditModel> GetByName(string name)
            => await _context.Subreddits
                .Where(s => s.Name == name)
                .FirstOrDefaultAsync();
    }
}