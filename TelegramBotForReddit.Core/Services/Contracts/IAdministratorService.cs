using System.Threading.Tasks;
using TelegramBotForReddit.Core.Dto.Administrator;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface IAdministratorService
    {
        Task<AdministratorDto> GetByAdminId(long id);
        Task<bool> IsUserAdmin(long id);
        Task<AdministratorDto> Delete(long id);
        Task<AdministratorDto> Create(long id);
        Task<bool?> IsUserSuperAdmin(long id);
    }
}