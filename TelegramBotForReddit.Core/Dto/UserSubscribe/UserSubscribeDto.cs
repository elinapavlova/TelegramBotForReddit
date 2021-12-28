using System;

namespace TelegramBotForReddit.Core.Dto.UserSubscribe
{
    public class UserSubscribeDto
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string SubredditName { get; set; }
        public DateTime DateSubscribed { get; set; }
        public DateTime? DateUnsubscribed { get; set; }
    }
}