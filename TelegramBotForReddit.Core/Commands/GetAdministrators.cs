using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class GetAdministrators : BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IAdministratorService _administratorService;
        private readonly IUserService _userService;
        private readonly TelegramHttpClient _telegramHttpClient;
        
        public GetAdministrators
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
        {
            var text = await CreateMessage();
            await _telegramHttpClient.SendTextMessage(message.Chat.Id, text);
        }

        private async Task<string> CreateMessage()
        {
            var administrators = await GetAdministratorIdsNames();
            var message = string.Empty;

            foreach (var (id, name) in administrators)
            {
                if (name != null)
                {
                    message += $"{id} [@{name}]\r\n";
                }
                else
                {
                    message += $"{id}\r\n";
                }
            }

            return message;
        }

        private async Task<Dictionary<long, string>> GetAdministratorIdsNames()
        {
            var ids = await _administratorService.GetAdministratorIds();
            var userNames = new Dictionary<long, string>();
            foreach (var id in ids)
            {
                var user = await _userService.GetById(id);
                if (user != null)
                {
                    userNames.Add(id, user.UserName);
                }
            }
            return userNames;
        }
    }
}