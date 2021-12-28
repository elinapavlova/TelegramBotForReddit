using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.User;

namespace TelegramBotForReddit.Core.Services.User
{
    public interface IUserService
    {
        Task<UserDto> Create(UserDto newUser);
        Task<UserDto> GetById(long id);
    }
}