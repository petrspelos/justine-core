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

    public static class DiggingJobExtensions
    {
        public static bool IsComplete(this DiggingJob job)
        {
#if DEBUG
            // In DEBUG, we need to wait for SECONDS instead of HOURS
            // to be able to test the feature without actually waiting.
            var finishDateTime = job.StartDateTime.AddSeconds(job.DiggingLengthInHours);
            //var finishDateTime = job.StartDateTime.AddSeconds(20);
#else
            var finishDateTime = job.StartDateTime.AddHours(job.DiggingLengthInHours);
#endif
            return finishDateTime < DateTime.Now;
        }

        public static int GetReward(this DiggingJob job)
        {
            return job.DiggingLengthInHours * Constants.DiggingGoldPerHour;
        }
    }
}
