using System.Threading.Tasks;
using AutoMapper;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Database.Repositories.Administrator;

namespace TelegramBotForReddit.Core.Services.Administrator
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IMapper _mapper;

        public AdministratorService
        (
            IAdministratorRepository administratorRepository,
            IMapper mapper
        )
        {
            _administratorRepository = administratorRepository;
            _mapper = mapper;
        }

        public async Task<AdministratorDto> GetByUserId(long id) 
            => _mapper.Map<AdministratorDto>(await _administratorRepository.GetByUserId(id));
        
        public async Task Delete(long id)
        { 
            await _administratorRepository.Delete(id);
        }
        
        public async Task<bool> IsUserAdmin(long id)
        {
            var admin = await GetByUserId(id);
            return admin != null;
        }
    }
}