using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.User;

namespace TelegramBotForReddit.Core.Commands
{
    public class MakeAdminCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserService _userService;
        private readonly ILogger<MakeAdminCommand> _logger;

        public MakeAdminCommand
        (
            string commandName,
            IAdministratorService administratorService,
            IUserService userService,
            ILogger<MakeAdminCommand> logger
        ) 
            : base(commandName)
        {
            Name = commandName;
            _administratorService = administratorService;
            _userService = userService;
            _logger = logger;
        }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
            => await client.SendTextMessageAsync (message.Chat.Id, await CreateMessage(message)); 

        private async Task<string> CreateMessage(Message message)
        {
            if (message.Text.Split(' ').Length != 2)
                return "Необходимо указать команду в виде /make_admin username";

            var fromId = message.From.Id;
            var userName = message.Text.Split(' ')[1];
            if (message.From.Username == userName)
                return "Необходимо указать имя другого пользователя.";
            
            var isUserSuperAdmin = await IsUserSuperAdmin(fromId);
            if (isUserSuperAdmin is null or false)
                return "Команда доступна только суперадминистраторам.";

            var user = await GetUserByName(userName);
            if (user == null)
                return $"Пользователь {userName} не найден.\r\nПримечание:" +
                       "\r\nНе используйте символ @ в начале имени." +
                       "\r\nЕсли пользователь изменил имя, ему необходимо перезапустить бот и повторить попытку.";

            var isActual = await IsUserActual(user.Id);
            if (isActual is null or false)
                return $"Пользователь {userName} не использует бот.";
            
            var isUserAdmin = await IsUserAdmin(user.Id);
            if (isUserAdmin)
                return $"Пользователь {userName} уже является администратором.";

            var admin = await MakeAdministrator(user.Id);
            _logger.LogInformation($"user {fromId} made admin user {userName}");
            
            return admin == null 
                ? $"Не удалось назначить администратором пользователя {userName}." 
                : $"Пользователь {userName} успешно назначен администратором.";
        }
        
        private async Task<bool?> IsUserSuperAdmin(long userId)
            => await _administratorService.IsUserSuperAdmin(userId);
        
        private async Task<bool> IsUserAdmin(long userId)
            => await _administratorService.IsUserAdmin(userId);
        
        private async Task<UserDto> GetUserByName(string userName)
            => await _userService.GetByName(userName);
        
        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);

        private async Task<AdministratorDto> MakeAdministrator(long userId)
            => await _administratorService.Create(userId);
    }
}