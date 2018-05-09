using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustineCore.Entities;
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

        public T RestoreObject<T>(string group, string key)
        {
            return RestoreObject<T>($"{group}.{key}");
        }

        public IEnumerable<T> RestoreGroup<T>(string group)
        {
            return _storage.Where(e => e.Key.StartsWith(group)).Select(e => (T)e.Value);
        }

        public void DeleteObject(string key)
        {
            if (!_storage.ContainsKey(key)) return;
            _storage.Remove(key);
        }

        public void DeleteObject(string group, string key)
        {
            DeleteObject($"{group}.{key}");
        }

        public IEnumerable<JustineLanguage> GetLanguages()
        {
            throw new NotImplementedException();
        }

        public void StoreObject(object obj, string group, string key)
        {
            StoreObject(obj, $"{group}.{key}");
        }
    }
}
