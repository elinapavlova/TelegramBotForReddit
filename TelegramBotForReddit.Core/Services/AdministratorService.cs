using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Services.Contracts;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public AdministratorService
        (
            IAdministratorRepository administratorRepository,
            IMapper mapper,
            IUserService userService
        )
        {
            _administratorRepository = administratorRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<AdministratorDto> GetByAdminId(long id) 
            => _mapper.Map<AdministratorDto>(await _administratorRepository.GetByAdminId(id));
        
        public async Task<AdministratorDto> Delete(long id)
            => _mapper.Map<AdministratorDto>(await _administratorRepository.Delete(id));

        public async Task<AdministratorDto> Create(long id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return null;

            var userAdmin = await GetByAdminId(id);
            if (userAdmin != null)
                return userAdmin;

            var admin = new AdministratorModel
            {
                UserId = id
            };
            
            var result = _mapper.Map<AdministratorDto>(await _administratorRepository.Create(admin));
            return result;
        }

        public async Task<bool> IsUserAdmin(long id)
        {
            var admin = await GetByAdminId(id);
            return admin != null;
        }
        
        public async Task<bool?> IsUserSuperAdmin(long id)
            => await _administratorRepository.IsUserSuperAdmin(id);
    }
}