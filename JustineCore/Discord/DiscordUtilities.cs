using System.Collections.Generic;
using Discord;

namespace JustineCore.Discord
{
    public static class DiscordUtilities
    {
        public static LogSeverity TryParseLogSeverityByKey(this Dictionary<string, string> d, string key)
        {
            if(!d.ContainsKey(key)) return LogSeverity.Info;
            return DiscordSocketConfigFactory.StringToLogSeverity(d[key]);
        }
    }
}