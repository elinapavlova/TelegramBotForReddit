using System.Collections.Generic;

namespace TelegramBotForReddit.Core.Options
{
    public class SmtpClientOptions
    {
        public const string SmtpClient = "SmtpClient";
        public string Host { get; set; }
        public int Port { get; set; }
        public string SslProtocol { get; set; }
        public bool EnableSsl { get; set; }
        public Dictionary<string, string> Credentials { get; set; }
    }
}