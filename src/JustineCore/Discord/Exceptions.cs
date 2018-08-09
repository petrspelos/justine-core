using System;

namespace JustineCore.Discord
{
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message)
            : base(message)
        {
        }

        public InvalidTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    internal class TokenNotSetException : Exception
    {
        public TokenNotSetException()
        {
        }

        public TokenNotSetException(string message)
            : base(message)
        {
        }

        public TokenNotSetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
