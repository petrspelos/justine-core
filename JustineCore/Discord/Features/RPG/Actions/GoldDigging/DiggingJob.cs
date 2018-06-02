using System;

namespace JustineCore.Discord.Features.RPG.GoldDigging
{
    public class DiggingJob
    {
        public ulong UserId { get; set; }
        public int DiggingLengthInHours { get; set; }
        public DateTime StartDateTime { get; set; }
        public ulong GuildId { get; set; }
        public ulong TextChannelId { get; set; }
    }
}
