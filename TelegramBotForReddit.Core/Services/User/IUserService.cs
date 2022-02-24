using System;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.User;

namespace TelegramBotForReddit.Core.Services.User
{
    public interface IUserService
    {
        Task<UserDto> Create(UserDto newUser);
        Task<int> Count();
        Task<int> GetCountOfStopsBotByDate(DateTime date);
        Task<int> GetCountOfStartsBotByDate(DateTime date);
        Task<UserDto> Get(long id);
        Task<bool?> IsActual(long id);
        Task StopBot(long id);
    }
}