using Discord;
using Discord.WebSocket;

namespace JustineCore.Discord
{
    public static class DiscordSocketConfigFactory
    {
        public static DiscordSocketConfig GetDefault()
        {
            return new DiscordSocketConfig
            {
                MessageCacheSize = 0,
                LogLevel = LogSeverity.Verbose,
            };
        }
    }
}