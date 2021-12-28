﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.UserSubscribe;

namespace TelegramBotForReddit.Core.Services.UserSubscribe
{
    public interface IUserSubscribeService
    {
        Task<UserSubscribeDto> Subscribe(long userId, string subredditName);
        Task<UserSubscribeDto> Unsubscribe(Guid id);
        Task<UserSubscribeDto> GetActual(long userId, string subredditName);
        Task<List<UserSubscribeDto>> GetByUserId(long userId);
        Task<List<UserSubscribeDto>> GetBySubredditName(string name);
    }
}