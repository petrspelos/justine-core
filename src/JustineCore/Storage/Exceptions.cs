using System;

namespace JustineCore.Storage
{
    public class DataStorageKeyDoesNotExistException : Exception
    {
        public DataStorageKeyDoesNotExistException()
        {
        }

        public DataStorageKeyDoesNotExistException(string message)
            : base(message)
        {
        }

        public DataStorageKeyDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DataStorageGroupDoesNotExistException : Exception
    {
        public DataStorageGroupDoesNotExistException()
        {
        }

        public DataStorageGroupDoesNotExistException(string message)
            : base(message)
        {
        }

        public DataStorageGroupDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    internal class LanguageNotFoundException : Exception
    {
        public LanguageNotFoundException()
        {
        }

        public LanguageNotFoundException(string message)
            : base(message)
        {
        }

        public LanguageNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    internal class LanguageResourceKeyNotFoundException : Exception
    {
        public LanguageResourceKeyNotFoundException()
        {
        }

        public LanguageResourceKeyNotFoundException(string message)
            : base(message)
        {
        }

        public LanguageResourceKeyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
