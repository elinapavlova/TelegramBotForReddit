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
        {
            var admin = _mapper.Map<AdministratorDto>(await _administratorRepository.GetByUserId(id));
            return admin;
        }
    }
}