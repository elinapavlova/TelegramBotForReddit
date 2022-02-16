using System;
using System.Collections.Generic;

namespace TelegramBotForReddit.Database.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime? DateStopped { get; set; }
        
        public List<UserSubscribeModel> Subscribes { get; set; }
    }
}