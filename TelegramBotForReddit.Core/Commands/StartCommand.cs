using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class StartCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private static Dictionary<string, string> _commands;
        private readonly IUserService _userService;
        private readonly TelegramHttpClient _telegramHttpClient;
        private readonly IMapper _mapper;

        public StartCommand
        (
            string commandName, 
            CommandsOptions options, 
            IUserService userService, 
            TelegramHttpClient telegramHttpClient,
            IMapper mapper
        ) 
            : base(commandName)
        {
            Name = commandName;
            _commands = options.Commands;
            _userService = userService;
            _telegramHttpClient = telegramHttpClient;
            _mapper = mapper;
        }

        public override async Task Execute(Message message)
        {             
            await CreateUser(message.From);
            await _telegramHttpClient.SendTextMessage(message.Chat.Id, CreateCommandsMessage());
        }

        private async Task CreateUser(User userFrom)
        {
            var user = _mapper.Map<UserDto>(userFrom);
            await _userService.CreateOrUpdate(user);
            Logger.Logger.LogInfo($"user {user.Id} [{user.UserName}] (re)started bot");
        }

        private static string CreateCommandsMessage()
        {
            var content = "Список доступных команд:";
            
            foreach (var command in _commands)
            {
                content += "\r\n";
                content += command.Value;
            }

            return content;
        }
    }
}