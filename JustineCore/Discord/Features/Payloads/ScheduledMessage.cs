using System.Collections.Generic;

namespace JustineCore.Discord.Features.Payloads
{
    public class ScheduledMessage
    {
        public List<MessagePayload> Payloads { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}