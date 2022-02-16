using System;

namespace TelegramBotForReddit.Core.Dto.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime? DateStopped { get; set; }
    }
}