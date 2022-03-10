using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> Create(UserModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<UserModel> Update(UserModel user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<int> Count()
            => await _context.Users
                .Where(us => us.DateStopped == null)
                .CountAsync();

        public async Task<int> GetCountOfStopsBotByDate(DateTime date)
            => await _context.Users
                .Where(us => us.DateStopped >= date)
                .CountAsync();

        public async Task<int> GetCountOfStartsBotByDate(DateTime date)
            => await _context.Users
                .Where(us => us.DateStarted >= date && us.DateStopped == null)
                .CountAsync();

        public async Task<UserModel> GetById(long id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        
        public async Task<UserModel> GetByName(string name)
            => await _context.Users.FirstOrDefaultAsync(u => u.UserName == name);

        public async Task<bool?> IsActual(long id)
        {
            var user = await GetById(id);
            if (user == null)
                return null;

            var isActual = await _context.Users.AnyAsync(u => u.Id == id && u.DateStopped == null);
            return isActual;
        }
    }
}