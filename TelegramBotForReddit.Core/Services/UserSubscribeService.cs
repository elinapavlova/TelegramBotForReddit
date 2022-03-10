using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.Subreddit;
using TelegramBotForReddit.Core.Dto.UserSubscribe;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Core.Services
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

        public async Task UnsubscribeAll(long userId)
        {
            var subscribes = await _userSubscribeRepository.GetByUserId(userId);
            foreach (var subscribe in subscribes)
            {
                subscribe.DateUnsubscribed = DateTime.Now;
                await _userSubscribeRepository.Update(subscribe);
            }
        }

        public async Task<UserSubscribeDto> GetActual(long userId, string subredditName)
            => _mapper.Map<UserSubscribeDto>(await _userSubscribeRepository.Get(userId, subredditName, true));

        public async Task<List<UserSubscribeDto>> GetByUserId(long userId)
            => _mapper.Map<List<UserSubscribeDto>>(await _userSubscribeRepository.GetByUserId(userId));

        public async Task<List<UserSubscribeDto>> GetBySubredditName(string name)
            => _mapper.Map<List<UserSubscribeDto>>(await _userSubscribeRepository.GetBySubredditName(name));

        public async Task<List<SubredditStatisticsDto>> GetPopularestSubreddits()
            => _mapper.Map<List<SubredditStatisticsDto>>(await _userSubscribeRepository.GetPopularestSubreddits());

        public async Task<int> GetAverageNumberOfSubscribes()
            => await _userSubscribeRepository.GetAverageNumberOfSubscribes();
    }
}