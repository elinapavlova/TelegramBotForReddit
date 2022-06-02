using System.Threading.Tasks;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface ISmtpSender
    {
        Task SendMessage(string text);
    }
}