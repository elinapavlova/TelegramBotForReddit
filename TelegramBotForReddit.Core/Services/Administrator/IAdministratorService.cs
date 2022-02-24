﻿using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.Administrator;

namespace TelegramBotForReddit.Core.Services.Administrator
{
    public interface IAdministratorService
    {
        Task<AdministratorDto> GetByUserId(long id);
        Task<bool> IsUserAdmin(long id);
        Task Delete(long id);
    }
}