using System;

namespace JustineCore.Discord.Features.RPG.Gold
{
    public class NotEnoughGoldException : Exception
    {
        public NotEnoughGoldException()
        {
        }

        public NotEnoughGoldException(string message)
            : base(message)
        {
        }

        public NotEnoughGoldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}