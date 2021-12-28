using System;

namespace TelegramBotForReddit.Database.Models
{
    public class UserSubscribeModel
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string SubredditName { get; set; }
        public DateTime DateSubscribed { get; set; }
        public DateTime? DateUnsubscribed { get; set; }
        
        public UserModel User { get; set; }
    }
}