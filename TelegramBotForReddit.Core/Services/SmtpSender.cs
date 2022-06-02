using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TelegramBotForReddit.Core.Options;
using TelegramBotForReddit.Core.Services.Contracts;

namespace TelegramBotForReddit.Core.Services
{
    public class SmtpSender : ISmtpSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _fromAddress;
        private readonly string _password;
        private readonly string _sslProtocol;
        private const string ToAddress = "elina.pavlova.02@mail.ru";

        public SmtpSender(IOptions<SmtpClientOptions> smtpOptions)
        {
            _host = smtpOptions.Value.Host;
            _port = smtpOptions.Value.Port;
            _enableSsl = smtpOptions.Value.EnableSsl;
            _sslProtocol = smtpOptions.Value.SslProtocol;
            smtpOptions.Value.Credentials.TryGetValue("Username", out var fromAddress);
            _fromAddress = fromAddress;
            smtpOptions.Value.Credentials.TryGetValue("Password", out var password);
            _password = password;
        }

        public async Task SendMessage(string text)
        {
            using var client = new SmtpClient();
            try
            {
                Enum.TryParse(_sslProtocol, out SslProtocols sslProtocols);
                client.SslProtocols = sslProtocols;
                
                await client.ConnectAsync(_host, _port, _enableSsl);
                await client.AuthenticateAsync(_fromAddress, _password);
                await client.SendAsync(Message("RedditBot Error", text));
                
                await client.DisconnectAsync(true);
                
                Logger.Logger.LogError($"Sent Message < {text} > from {_fromAddress} to {ToAddress}. Date {DateTime.Now}");
            }
            catch(Exception exception)
            {
                Logger.Logger.LogError($"Error while sending message: {exception.Message}");
            }
        }
        
        private MimeMessage Message(string subject, string text)
        {
            var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress("", _fromAddress));
            emailMessage.To.Add(new MailboxAddress("", ToAddress));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Text)
            {
                Text = text
            };

            return emailMessage;
        }
    }
}