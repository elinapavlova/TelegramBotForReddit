using System;

namespace TelegramBotForReddit.Database.Models
{
    public class AdministratorModel
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}