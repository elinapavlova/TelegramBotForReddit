using System;
using System.IO;
using NLog.Web;

namespace TelegramBotForReddit.Core.Logger
{
    public static class Logger
    {
        private static readonly NLog.Logger _logger = NLogBuilder.ConfigureNLog(Path.GetFullPath("nlog.config")).GetCurrentClassLogger();

        public static void LogError(string message)
        {
            _logger.Error(message);
            Console.WriteLine(message);
        }
        
        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }
    }
}