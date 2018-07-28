using System;

namespace JustineCore.Entities
{
    public class UserProblemView
    {
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}