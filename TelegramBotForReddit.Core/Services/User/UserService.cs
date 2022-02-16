using System;
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
            var user = await _userRepository.Get(newUser.Id, false);
            if (user != null)
            {
                user.DateStarted = DateTime.Now;
                user.DateStopped = null;
                return _mapper.Map<UserDto>(await _userRepository.Update(user));
            }
            
            newUser.DateStarted = DateTime.Now;
            newUser.DateStopped = null;
            user = _mapper.Map<UserModel>(newUser);
            
            var result = _mapper.Map<UserDto>(await _userRepository.Create(user));
            return result;
        }

        public async Task<int> GetCountOfStopsBotByDate(DateTime date)
            => await _userRepository.GetCountOfStopsBotByDate(date);
        
        public async Task<UserDto> Get(long id, bool isActual)
            => _mapper.Map<UserDto>(await _userRepository.Get(id, isActual));

        public async Task StopBot(long id)
        {
            // Добавить проверку на администратора и удалить запись, если является администратором
            var user = await _userRepository.Get(id, true);
            if (user != null)
            {
                user.DateStopped = DateTime.Now;
                await _userRepository.Update(user);
            }
        }
    }
}