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

        public void Store(object obj, string key)
        {
            if (_storage.ContainsKey(key))
                _storage[key] = obj;
            else
                _storage.Add(key, obj);
        }

        public T Get<T>(string key)
        {
            if (_storage.ContainsKey(key))
                return (T)_storage[key];

            throw new DataStorageKeyDoesNotExistException(
                $"No object stored with the '{key}' key."
            );
        }

        public T Get<T>(string group, string key)
        {
            return Get<T>($"{group}.{key}");
        }

        public IEnumerable<T> GetGroup<T>(string group)
        {
            return _storage.Where(e => e.Key.StartsWith(group)).Select(e => (T)e.Value);
        }

        public void Delete(string key)
        {
            if (!_storage.ContainsKey(key)) return;
            _storage.Remove(key);
        }

        public void Delete(string group, string key)
        {
            Delete($"{group}.{key}");
        }

        public IEnumerable<JustineLanguage> GetLanguages()
        {
            throw new NotImplementedException();
        }

        public void Store(object obj, string group, string key)
        {
            Store(obj, $"{group}.{key}");
        }
    }
}
