using System.Drawing;
using System.Threading.Tasks;

namespace TelegramBotForReddit.Core.Services.Contracts
{
    public interface IWordCloudService
    {
        Task<Image> Draw();
    }
}