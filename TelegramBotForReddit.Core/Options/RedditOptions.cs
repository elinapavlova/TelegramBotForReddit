namespace TelegramBotForReddit.Core.Options
{
    public class RedditOptions
    {
        public const string Reddit = "Reddit";
        public string Id { get; set; }
        public string Secret { get; set; }
        public string BaseAddress { get; set; }
        public string RefreshToken { get; set; }
    }
}