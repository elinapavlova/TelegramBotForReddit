using System.IO;
using Microsoft.Extensions.Configuration;

namespace TelegramBotForReddit.Core.Configurations
{
    public static class ConfigurationBuilder
    {
        private static readonly IConfigurationRoot Builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();

        public static IConfigurationRoot Build() 
            => Builder;
    }
}