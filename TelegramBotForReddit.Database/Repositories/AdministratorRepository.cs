using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Database.Repositories
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<AdministratorModel> GetByAdminId(long id)
            => await _context.Administrators
                .Where(a => a.UserId == id)
                .FirstOrDefaultAsync();

        public async Task<AdministratorModel> Delete(long id)
        {
            var user = await GetByAdminId(id);
            if (user == null)
                return null;
            
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<AdministratorModel> Create(AdministratorModel user)
        {
            await _context.Administrators.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<bool?> IsUserSuperAdmin(long id)
        {
            var user = await GetByAdminId(id);
            return user?.IsSuperAdmin;
        }
    }
}