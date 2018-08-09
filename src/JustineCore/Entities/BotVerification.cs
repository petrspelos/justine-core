namespace JustineCore.Entities
{
    public class BotVerification
    {
        public ulong BotId { get; set; }
        public ulong OwnerId { get; set; }
        public bool Verified { get; set; }
        public string VerificationString { get; set; }
    }
}