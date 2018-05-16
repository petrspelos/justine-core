using System.Collections.Generic;
using JustineCore.Discord.Features.Payloads;

namespace JustineCore.Entities
{
    public class DiscordBotConfig
    {
        public string Token { get; set; }
        public List<ScheduledMessage> ScheduledMessages { get; set; } = new List<ScheduledMessage>();
    }
}
