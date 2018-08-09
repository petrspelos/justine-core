using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace JustineCore.Discord
{
    public static class DiscordSocketConfigFactory
    {
        public static DiscordSocketConfig GetDefault()
        {
            return new DiscordSocketConfig
            {
                /* RestClientProvider = null, */
                /* UdpSocketProvider = null, */
                /* WebSocketProvider = null, */
                MessageCacheSize = 0,
                AlwaysDownloadUsers = false,
                /* --- */
                ConnectionTimeout = 0,
                LargeThreshold = 0,
                HandlerTimeout = null,
                ShardId = null,
                TotalShards = null,
                GatewayHost = "",
                LogLevel = LogSeverity.Verbose,
                DefaultRetryMode = RetryMode.AlwaysFail
            };
        }

        ///<summary>Creates a DiscordSocketConfig out of a json of a (string,string) dictionary</summary>
        public static DiscordSocketConfig FromJsonDictionary(string json)
        {
            var dictionary = new Dictionary<string, string>();

            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch(Exception)
            {
                throw new ArgumentException("The provided JSON is not in the correct format.", "json");
            }

            return new DiscordSocketConfig
            {
                MessageCacheSize = dictionary.TryParseIntByKey("MessageCacheSize"),
                AlwaysDownloadUsers = dictionary.TryParseBoolByKey("AlwaysDownloadUsers"),
                LogLevel = dictionary.TryParseLogSeverityByKey("LogLevel")
            };
        }

        public static LogSeverity StringToLogSeverity(string value)
        {
            try
            {
                var severity = (LogSeverity) Enum.Parse(typeof(LogSeverity), value);
                if(Enum.IsDefined(typeof(LogSeverity), severity)) return severity;
                throw new Exception($"{severity} is not an underlying value of the LogSeverity enumeration.");
            }
            catch(Exception)
            {
                return LogSeverity.Info;
            }
        }
    }
}