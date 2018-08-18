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
                // ConnectionTimeout = 0,
                LargeThreshold = 0,
                HandlerTimeout = null,
                ShardId = null,
                TotalShards = null,
                // GatewayHost = "",
                DefaultRetryMode = RetryMode.AlwaysRetry,
                LogLevel = LogSeverity.Debug
            };
        }

        public static DiscordSocketConfig FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<DiscordSocketConfig>(json);
            }
            catch(JsonReaderException)
            {
                return GetDefault();
            }
        }
    }
}
