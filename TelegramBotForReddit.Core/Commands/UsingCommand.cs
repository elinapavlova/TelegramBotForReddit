using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;

namespace TelegramBotForReddit.Core.Commands
{
    public class UsingCommand : BaseCommand
    {
        public sealed override string Name { get; init; }
        
        public UsingCommand(string commandName) : base(commandName)
        {
            Name = commandName;
        }

        public override async Task<Message> Execute(Message message, ITelegramBotClient client)
            => await client.SendTextMessageAsync (message.Chat.Id, CreateMessage(message.From.Username));

        private static string CreateMessage(string userName)
        {
            var text = "/start - запустить (перезапустить) бот\r\n\r\n" +
                            "/subscribe AskReddit - подписаться на AskReddit\r\n\r\n" +
                            "/unsubscribe AskReddit - отписаться от AskReddit\r\n\r\n" +
                            "/subscriptions - получить список подписок\r\n\r\n" +
                            "/subreddits - получить список доступных для подписки сабреддитов\r\n\r\n" +
                            "/using - получить примеры использования команд\r\n\r\n" +
                            "/statistics - получить статистику (доступно только администраторам\r\n\r\n" +
                            $"/make_admin {userName} - назначить пользователя администратором (доступно только администраторам)\r\n\r\n" +
                            $"/cancel_admin {userName} - отменить назначение пользователя администратором (доступно только администраторам)";
            return text;
        } 
    }
}