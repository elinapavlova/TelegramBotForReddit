using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class CancelAdminCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly ILogger<CancelAdminCommand> _logger;
        private readonly IAdministratorService _administratorService;
        private readonly IUserService _userService;
        
        public CancelAdminCommand
        (
            string commandName, 
            ILogger<CancelAdminCommand> logger, 
            IAdministratorService administratorService,
            IUserService userService
        ) : base(commandName)
        {
            Name = commandName;
            _logger = logger;
            _administratorService = administratorService;
            _userService = userService;
        }
        
        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
            => await client.SendTextMessageAsync(message.Chat.Id, await CreateMessage(message));

        private async Task<string> CreateMessage(Message message)
        { 
            if (message.Text.Split(' ').Length != 2)
                return "Необходимо указать команду в виде /cancel_admin username";

            var fromId = message.From.Id;
            var fromName = message.From.Username;
            var userName = message.Text.Split(' ')[1];
            if (message.From.Username == userName)
                return "Необходимо указать имя другого пользователя.";
            
            var isUserAdmin = await IsUserAdmin(fromId);
            if (!isUserAdmin)
                return "Команда доступна только администраторам.";

            var user = await GetUserByName(userName);
            if (user == null)
                return $"Пользователь {userName} не найден.\r\nПримечание:" +
                       "\r\nНе используйте символ @ в начале имени." +
                       "\r\nЕсли пользователь изменил имя, ему необходимо перезапустить бот и повторить попытку.";

            var isActual = await IsUserActual(user.Id);
            if (isActual is null or false)
                return $"Пользователь {userName} не использует бот.";
            
            isUserAdmin = await IsUserAdmin(user.Id);
            if (!isUserAdmin)
                return $"Пользователь {userName} не является администратором.";

            var admin = await CancelAdministrator(user.Id);
            _logger.LogInformation($"user {fromId} [{fromName}] cancel admin user {user.Id} [{userName}]");
            
            return admin == null 
                ? $"Не удалось отменить назначение администратором пользователя {userName}." 
                : $"Назначение администратором пользователя {userName} отменено.";
        }
        
        private async Task<bool> IsUserAdmin(long userId)
            => await _administratorService.IsUserAdmin(userId);
        
        private async Task<UserDto> GetUserByName(string userName)
            => await _userService.GetByName(userName);
        
        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);

        private async Task<AdministratorDto> CancelAdministrator(long userId)
            => await _administratorService.Delete(userId);
    }
}