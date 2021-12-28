using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.User;

namespace TelegramBotForReddit.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Create(UserDto newUser)
        {
            var user = _mapper.Map<UserModel>(newUser);
            var result = _mapper.Map<UserDto>(await _userRepository.Create(user));
            return result;
        }

        public async Task<UserDto> GetById(long id)
        {
            var result = _mapper.Map<UserDto>(await _userRepository.GetById(id));
            return result;
        }
    }
}