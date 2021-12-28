using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotForReddit.Core.Commands.Base
{
    public abstract class BaseCommand
    {
        protected BaseCommand(string commandName)
        {
            Name = commandName;
        }

        public abstract string Name { get; init; }
        public abstract Task<Message> Execute(Message message, ITelegramBotClient botClient);
        public bool Contains(string message)
            => Name.Equals(message);
    }
}