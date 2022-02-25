using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Services.User;
using TelegramBotForReddit.Database.Models;
using TelegramBotForReddit.Database.Repositories.Administrator;

namespace TelegramBotForReddit.Core.Services.Administrator
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

        public async Task<AdministratorDto> GetByUserId(long id) 
            => _mapper.Map<AdministratorDto>(await _administratorRepository.GetByUserId(id));
        
        public async Task Delete(long id)
        { 
            await _administratorRepository.Delete(id);
        }

        public async Task<AdministratorDto> Create(long id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return null;

            var userAdmin = await GetByUserId(id);
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
            var admin = await GetByUserId(id);
            return admin != null;
        }
    }
}