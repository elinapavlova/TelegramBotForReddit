using System;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.User;

namespace TelegramBotForReddit.Core.Services.User
{
    public interface IUserService
    {
        Task<UserDto> Create(UserDto newUser);
        Task<int> GetCountOfStopsBotByDate(DateTime date);
        Task<UserDto> Get(long id, bool isActual);
        Task StopBot(long id);
    }
}