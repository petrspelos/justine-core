using System.Collections.Generic;
using JustineCore.Discord.Features.Payloads;

namespace JustineCore.Entities
{
    public class GlobalUserData
    {
        public ulong DiscordId { get; set; }
        public DataConsent CollectionConsent { get; set; }
    }
}
