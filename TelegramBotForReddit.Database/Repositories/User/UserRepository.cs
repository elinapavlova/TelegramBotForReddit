using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.User
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

        public async Task<int> GetCountOfStopsBotByDate(DateTime date)
            => await _context.Users
                .Where(us => us.DateStopped >= date)
                .CountAsync();
        
        public async Task<UserModel> Get(long id, bool isActual)
        {
            return isActual
                ? await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == id
                    && u.DateStopped == null)
                : await _context.Users.FirstOrDefaultAsync(u =>
                    u.Id == id
                    && u.DateStopped != null);
        }
    }
}