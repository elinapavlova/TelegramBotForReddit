using System.Collections.Generic;

namespace TelegramBotForReddit.Database.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        
        public List<UserSubscribeModel> Subscribes { get; set; }
    }
}