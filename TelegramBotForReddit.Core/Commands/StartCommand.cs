using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NLog;
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
        {
            var u = _mapper.Map<UserDto>(message.From);
            var user = await _userService.GetById(u.Id) ?? await _userService.Create(u);
            var content = CreateMessage(); 
            _logger.LogInformation($"user {user.Id} (re)started bot");
            return await client.SendTextMessageAsync (message.Chat.Id, content); 
        }
        
        private static string CreateMessage()
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