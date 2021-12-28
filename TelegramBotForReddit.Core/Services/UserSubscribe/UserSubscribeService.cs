using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.UserSubscribe;

namespace TelegramBotForReddit.Core.Services.UserSubscribe
{
    public class UserSubscribeService : IUserSubscribeService
    {
        private readonly IUserSubscribeRepository _userSubscribeRepository;
        private readonly IMapper _mapper;
        
        public UserSubscribeService(IUserSubscribeRepository userSubscribeRepository, IMapper mapper)
        {
            _userSubscribeRepository = userSubscribeRepository;
            _mapper = mapper;
        }

        public async Task<UserSubscribeDto> Subscribe(long userId, string subredditName)
        {
            var userSubscribe = await _userSubscribeRepository.Get(userId, subredditName, false);
            if (userSubscribe != null)
            {
                userSubscribe.DateSubscribed = DateTime.Now;
                userSubscribe.DateUnsubscribed = null;
                return _mapper.Map<UserSubscribeDto>(await _userSubscribeRepository.Update(userSubscribe));
            }
            
            userSubscribe = new UserSubscribeModel
            {
                UserId = userId,
                SubredditName = subredditName,
                DateSubscribed = DateTime.Now,
                DateUnsubscribed = null
            };

            var result = _mapper.Map<UserSubscribeDto>(await _userSubscribeRepository.Create(userSubscribe));
            return result;
        }

        public async Task<UserSubscribeDto> Unsubscribe(Guid id)
        {
            var userSubscribe = await _userSubscribeRepository.GetById(id);
            userSubscribe.DateUnsubscribed = DateTime.Now;
            var result = _mapper.Map<UserSubscribeDto>(await _userSubscribeRepository.Update(userSubscribe));
            return result;
        }

        public async Task<UserSubscribeDto> GetActual(long userId, string subredditName)
        {
            var result = _mapper.Map<UserSubscribeDto>(await _userSubscribeRepository.Get(userId, subredditName, true));
            return result;
        }

        public async Task<List<UserSubscribeDto>> GetByUserId(long userId)
        {
            var result = _mapper.Map<List<UserSubscribeDto>>(await _userSubscribeRepository.GetByUserId(userId));
            return result;
        }

        public async Task<List<UserSubscribeDto>> GetBySubredditName(string name)
        {
            var result = _mapper.Map<List<UserSubscribeDto>>(await _userSubscribeRepository.GetBySubredditName(name));
            return result;
        }
    }
}