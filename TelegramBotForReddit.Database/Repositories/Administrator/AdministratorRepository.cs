using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.Administrator
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<AdministratorModel> GetByUserId(long id)
            => await _context.Administrators
                .Where(a => a.UserId == id)
                .FirstOrDefaultAsync();
    }
}