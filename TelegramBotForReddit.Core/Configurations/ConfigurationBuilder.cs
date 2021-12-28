using Microsoft.Extensions.Configuration;

namespace TelegramBotForReddit.Core.Configurations
{
    public static class ConfigurationBuilder
    {
        private const string Path = "";
        
        private static readonly IConfigurationRoot Builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddJsonFile(Path, false)
            .Build();

        public static IConfigurationRoot Build() 
            => Builder;
    }
}