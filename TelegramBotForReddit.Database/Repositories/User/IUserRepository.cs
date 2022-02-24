using System;
using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserModel> Create(UserModel user);
        Task<UserModel> Update(UserModel user);
        Task<int> Count();
        Task<int> GetCountOfStopsBotByDate(DateTime date);
        Task<int> GetCountOfStartsBotByDate(DateTime date);
        Task<UserModel> Get(long id);
        Task<bool?> IsActual(long id);
    }
}