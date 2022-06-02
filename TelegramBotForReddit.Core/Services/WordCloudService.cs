using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sdcb.WordClouds;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class WordCloudService : IWordCloudService
    {
        private readonly IUserSubscribeService _userSubscribeService;

        public WordCloudService(IUserSubscribeService userSubscribeService)
        {
            _userSubscribeService = userSubscribeService;
        }
        
        public async Task<Image> Draw()
        {
            var wordCloud = new WordCloud(1024, 768);
            var wordsCount = await _userSubscribeService.GetSubscribesNameCount();
            var freqs = wordsCount.Select(w => w.Count).ToList();
            var words = wordsCount.Select(w => w.Name).ToList();

            var image = wordCloud.Draw(words, freqs);
            return image;
        }
    }
}