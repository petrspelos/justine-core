using System;
using System.Collections.Generic;
using System.Text;
using JustineCore.Storage;

namespace JustineCore.Tests.Helpers
{
    public class InMemoryDataStorage : IDataStorage
    {
        private readonly Dictionary<string, object> _storage;

        public InMemoryDataStorage()
        {
            _storage = new Dictionary<string, object>();
        }

        public void StoreObject(object obj, string key)
        {
            if (_storage.ContainsKey(key))
                _storage[key] = obj;
            else
                _storage.Add(key, obj);
        }

        public T RestoreObject<T>(string key)
        {
            if (_storage.ContainsKey(key))
                return (T)_storage[key];

            throw new DataStorageKeyDoesNotExistException(
                $"No object stored with the '{key}' key."
            );
        }
    }
}
