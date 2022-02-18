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
            var result = new UserDto();
            var user = await _userRepository.Get(newUser.Id);
            var isActual = await _userRepository.IsActual(newUser.Id);
            
            switch (isActual)
            {
                case false:
                    user.DateStarted = DateTime.Now;
                    user.DateStopped = null;
                    result = _mapper.Map<UserDto>(await _userRepository.Update(user));
                    break;
                
                case null:
                {
                    newUser.DateStarted = DateTime.Now;
                    newUser.DateStopped = null;
                    user = _mapper.Map<UserModel>(newUser);

                    result = _mapper.Map<UserDto>(await _userRepository.Create(user));
                    break;
                }
            }
            return result;
        }

        public async Task<int> Count()
            => await _userRepository.Count();

        public async Task<int> GetCountOfStopsBotByDate(DateTime date)
            => await _userRepository.GetCountOfStopsBotByDate(date);

        public async Task<int> GetCountOfStartsBotByDate(DateTime date)
            => await _userRepository.GetCountOfStartsBotByDate(date);

        public async Task<UserDto> Get(long id)
            => _mapper.Map<UserDto>(await _userRepository.Get(id));

        public async Task<bool?> IsActual(long id)
            => await _userRepository.IsActual(id);

        public async Task StopBot(long id)
        {
            var user = await _userRepository.Get(id);
            var isActual = await _userRepository.IsActual(id);
            if (isActual == true)
            {
                user.DateStopped = DateTime.Now;
                await _userRepository.Update(user);
            }
        }
    }
}