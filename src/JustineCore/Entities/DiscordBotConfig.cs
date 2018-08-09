using JustineCore.Discord.Features.Payloads;
using System.Collections.Generic;

namespace JustineCore.Entities
{
    public class DiscordBotConfig
    {
        public string Token { get; set; }
        public List<ScheduledMessage> ScheduledMessages { get; set; } = new List<ScheduledMessage>();
    }
}
