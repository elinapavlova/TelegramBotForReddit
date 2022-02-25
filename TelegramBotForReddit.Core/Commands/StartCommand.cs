using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.Dto.User;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.User;

namespace TelegramBotForReddit.Core.Commands
{
    public class StartCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        private static Dictionary<string, string> _commands;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<StartCommand> _logger;
        
        public StartCommand
        (
            string commandName, 
            CommandsOptions options, 
            IUserService userService, 
            IMapper mapper,
            ILogger<StartCommand> logger
        ) 
            : base(commandName)
        {
            Name = commandName;
            _commands = options.Commands;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
            => await client.SendTextMessageAsync (message.Chat.Id, await CreateMessage(message));

        private async Task<string> CreateMessage(Message message)
        {
            await CreateUser(message.From);
            return CreateCommandsMessage();
        }

        private async Task CreateUser(User userFrom)
        {
            var user = _mapper.Map<UserDto>(userFrom);
            await _userService.Create(user);
            _logger.LogInformation($"user {user.Id} (re)started bot");
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