using System;
using System.Threading.Tasks;
using Discord;

namespace JustineCore.Discord
{
    internal static class Logger
    {
        internal static Task Log(LogMessage logMessage)
        {
            Console.ForegroundColor = SeverityToConsoleColor(logMessage.Severity);
            var message = $"{DateTime.Now.ToShortTimeString()} [{logMessage.Source}] {logMessage.Message}";
            Console.WriteLine(message);
            Console.ResetColor();
            return Task.CompletedTask;
        }

        private static ConsoleColor SeverityToConsoleColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Debug:
                    return ConsoleColor.Green;
                case LogSeverity.Error:
                    return ConsoleColor.Red;
                case LogSeverity.Info:
                    return ConsoleColor.Cyan;
                case LogSeverity.Verbose:
                    return ConsoleColor.White;
                case LogSeverity.Warning:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
