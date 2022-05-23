using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.Administrator;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class CancelAdminCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserService _userService;
        private readonly TelegramHttpClient _telegramHttpClient;
        
        public CancelAdminCommand
        (
            string commandName,
            IAdministratorService administratorService,
            IUserService userService,
            TelegramHttpClient telegramHttpClient
        ) : base(commandName)
        {
            Name = commandName;
            _administratorService = administratorService;
            _userService = userService;
            _telegramHttpClient = telegramHttpClient;
        }
        
        public override async Task Execute(Message message)
            => await _telegramHttpClient.SendTextMessage(message.Chat.Id, await CreateMessage(message));

        private async Task<string> CreateMessage(Message message)
        { 
            if (message.Text.Split(' ').Length != 2)
                return "Необходимо указать команду в виде /cancel_admin username";

            var fromId = message.From.Id;
            var fromName = message.From.Username;
            var userName = message.Text.Split(' ')[1];
            
            if (fromName == userName)
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
            
            var isUserSuperAdmin = await IsUserSuperAdmin(user.Id);
            if (isUserSuperAdmin is true)
                return $"Пользователь {userName} является суперадминистратором.";

            var admin = await CancelAdministrator(user.Id);
            if (admin is null) 
                return $"Не удалось отменить назначение администратором пользователя {userName}.";
            
            Logger.Logger.LogInfo($"user {fromId} [{fromName}] cancel admin user {user.Id} [{userName}]");
            return $"Назначение администратором пользователя {userName} отменено.";

        }
        
        private async Task<bool> IsUserAdmin(long userId)
            => await _administratorService.IsUserAdmin(userId);
        
        private async Task<UserDto> GetUserByName(string userName)
            => await _userService.GetByName(userName);
        
        private async Task<bool?> IsUserActual(long userId)
            => await _userService.IsActual(userId);

        private async Task<AdministratorDto> CancelAdministrator(long userId)
            => await _administratorService.Delete(userId);
        
        private async Task<bool?> IsUserSuperAdmin(long userId)
            => await _administratorService.IsUserSuperAdmin(userId);
    }
}