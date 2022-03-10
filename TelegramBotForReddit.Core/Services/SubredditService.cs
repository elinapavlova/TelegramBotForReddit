using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.Subreddit;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class SubredditService : ISubredditService
    {
        private readonly ISubredditRepository _subredditRepository;
        private readonly IMapper _mapper;

        public SubredditService
        (
            ISubredditRepository subredditRepository, 
            IMapper mapper
        )
        {
            _subredditRepository = subredditRepository;
            _mapper = mapper;
        }

        public async Task<List<SubredditDto>> GetAll()
            => _mapper.Map<List<SubredditDto>>(await _subredditRepository.GetAll());

        public async Task<SubredditDto> GetByName(string name)
            => _mapper.Map<SubredditDto>(await _subredditRepository.GetByName(name));

        public async Task<SubredditDto> Create(string name)
        {
            var subreddit = await GetByName(name);
            if (subreddit != null)
                return subreddit;

            var newSubreddit = new SubredditModel
            {
                Name = name
            };

            var result = _mapper.Map<SubredditDto>(await _subredditRepository.Create(newSubreddit));
            return result;
        }
    }
}