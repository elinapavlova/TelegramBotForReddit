﻿using System.Threading.Tasks;
using TelegramBotForReddit.Database.Models;

namespace TelegramBotForReddit.Database.Repositories.Administrator
{
    public interface IAdministratorRepository
    {
        Task<AdministratorModel> GetByAdminId(long id);
        Task<AdministratorModel> Delete(long id);
        Task<AdministratorModel> Create(AdministratorModel user);
        Task<bool?> IsUserSuperAdmin(long id);
    }
}