using System.Collections.Generic;

namespace TelegramBotForReddit.Core.Options
{
    public class CommandsOptions
    {
        public const string Command = "Command";
        public Dictionary<string, string> Commands { get; set; }
    }
}