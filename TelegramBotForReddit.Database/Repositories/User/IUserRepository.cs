using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserModel> Create(UserModel user);
        Task<UserModel> GetById(long id);
    }
}