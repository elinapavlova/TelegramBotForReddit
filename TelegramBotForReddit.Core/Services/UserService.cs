using System;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Core.Services
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

        public async Task<UserDto> CreateOrUpdate(UserDto newUser)
        {
            var result = new UserDto();
            var user = await _userRepository.GetById(newUser.Id);
            var isActual = await _userRepository.IsActual(newUser.Id);
            
            switch (isActual)
            {
                case false:
                    user.DateStarted = DateTime.Now;
                    user.DateStopped = null;
                    user.UserName = newUser.UserName;
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

        public async Task<UserDto> GetById(long id)
            => _mapper.Map<UserDto>(await _userRepository.GetById(id));
        
        public async Task<UserDto> GetByName(string name)
            => _mapper.Map<UserDto>(await _userRepository.GetByName(name));

        public async Task<bool?> IsActual(long id)
            => await _userRepository.IsActual(id);

        public async Task StopBot(long id)
        {
            var user = await _userRepository.GetById(id);
            var isActual = await _userRepository.IsActual(id);
            if (isActual == true)
            {
                user.DateStopped = DateTime.Now;
                await _userRepository.Update(user);
            }
        }
    }
}