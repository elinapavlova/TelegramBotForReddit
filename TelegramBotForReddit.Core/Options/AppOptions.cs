namespace TelegramBotForReddit.Core.Options
{
    public class AppOptions
    {
        public const string App = "App";
        public string BotToken { get; set; }
        public string RedditId { get; set; }
        public string RedditSecret { get; set; }
        public string RedditBaseAddress { get; set; }
        public string RedditRefreshToken { get; set; }
        public string BotName { get; set; }
    }
}