using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotForReddit.Core.Commands.Base;
using TelegramBotForReddit.Core.HttpClients;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Commands
{
    public class DrawWordCloudCommand: BaseCommand
    {
        public sealed override string Name { get; init; }
        private readonly IWordCloudService _wordCloudService;
        private readonly TelegramHttpClient _telegramHttpClient;
        
        public DrawWordCloudCommand
        (
            string commandName, 
            IWordCloudService wordCloudService,
            TelegramHttpClient telegramHttpClient
        ) : base(commandName)
        {
            Name = commandName;
            _wordCloudService = wordCloudService;
            _telegramHttpClient = telegramHttpClient;
        }
        
        public override async Task Execute(Message message)
        {
            var image = await _wordCloudService.Draw();
            image.Save("C:/wordcloud.png", ImageFormat.Png);
            var path = "C:/wordcloud.png";
            var filestream = new FileStream(path, FileMode.Open, FileAccess.Read);
            await _telegramHttpClient.SendPhotoMessage(message.Chat.Id, filestream, string.Empty);
        }
    }
}