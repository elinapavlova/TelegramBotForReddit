using System.Collections.Generic;

namespace TelegramBotForReddit.Core.Dto.RedditMedia
{
    public class Media
    {
        public string Url { get; set; }
        public List<Image> Images { get; set; }
        public bool Enabled { get; set; }
    }
}