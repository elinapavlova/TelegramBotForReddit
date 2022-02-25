using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Services.Administrator;
using TelegramBotForReddit.Core.Services.User;

namespace TelegramBotForReddit.Core.Commands
{
    public class MakeAdminCommand : BaseCommand
    {
        public override string Name { get; init; }
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
        {
            if (message.Text.Split(' ').Length != 2)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Необходимо указать команду в виде /make_admin username");

            var isUserAdmin = await _administratorService.IsUserAdmin(message.From.Id);
            if (!isUserAdmin)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Команда доступна только администраторам.");
            
            var userName = message.Text.Split(' ')[1];
            if (message.From.Username == userName)
                return await client.SendTextMessageAsync (message.Chat.Id, 
                    "Необходимо указать имя другого пользователя.");

            var text = await CreateMessage(message.From.Id, userName);
            return await client.SendTextMessageAsync (message.Chat.Id, text);
        }

        private async Task<string> CreateMessage(long fromId, string name)
        {
            var user = await _userService.GetByName(name);
            if (user == null)
                return $"Пользователь {name} не найден.\r\nПримечание:" +
                       "\r\nНе используйте символ @ в начале имени." +
                       "\r\nЕсли пользователь изменил имя, ему необходимо перезапустить бот и повторить попытку.";

            var isActual = await _userService.IsActual(user.Id);
            if (isActual == null || (bool) !isActual)
                return $"Пользователь {name} не использует бот.";
            
            var isUserAdmin = await _administratorService.IsUserAdmin(user.Id);
            if (isUserAdmin)
                return $"Пользователь {name} уже является администратором.";

            var admin = await _administratorService.Create(user.Id);
            _logger.LogInformation($"user {fromId} made admin user {name}");
            return admin == null 
                ? $"Не удалось назначить администратором пользователя {name}." 
                : $"Пользователь {name} успешно назначен администратором.";
        }
    }
}