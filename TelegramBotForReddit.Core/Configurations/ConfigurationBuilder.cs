using System.IO;
using Microsoft.Extensions.Configuration;

namespace TelegramBotForReddit.Core.Configurations
{
    public static class ConfigurationBuilder
    {
        private static readonly IConfigurationRoot Builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appsettings.default.json", false)
            .AddJsonFile("appsettings.json", true)
            .Build();

        public static IConfigurationRoot Build() 
            => Builder;
    }
}