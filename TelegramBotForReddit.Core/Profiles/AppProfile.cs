using AutoMapper;
using TelegramBotForReddit.Database.Models;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Dto.Subreddit;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.Dto.UserSubscribe;

namespace TelegramBotForReddit.Core.Profiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<UserDto, UserModel>();
            CreateMap<UserModel, UserDto>();
            CreateMap<User, UserDto>();

            CreateMap<UserSubscribeDto, UserSubscribeModel>();
            CreateMap<UserSubscribeModel, UserSubscribeDto>();

            CreateMap<AdministratorModel, AdministratorDto>();
            CreateMap<AdministratorDto, AdministratorModel>();

            CreateMap<SubredditStatisticsModel, SubredditStatisticsDto>();
            CreateMap<SubredditStatisticsDto, SubredditStatisticsModel>();

            CreateMap<SubredditModel, SubredditDto>();
            CreateMap<SubredditDto, SubredditModel>();
        }
    }
}