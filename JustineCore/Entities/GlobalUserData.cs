using JustineCore.Discord.Features.RPG;

namespace JustineCore.Entities
{
    public class GlobalUserData
    {
        public ulong DiscordId { get; set; }
        public DataConsent CollectionConsent { get; set; }
        public RpgAccount RpgAccount { get; set; } = new RpgAccount();
    }
}
