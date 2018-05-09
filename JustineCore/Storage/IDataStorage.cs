using System.Collections.Generic;
using JustineCore.Entities;

namespace JustineCore.Storage
{
    public interface IDataStorage
    {
        void StoreObject(object obj, string key);

        void StoreObject(object obj, string group, string key);

        T RestoreObject<T>(string key);

        T RestoreObject<T>(string group, string key);

        IEnumerable<T> RestoreGroup<T>(string group);

        void DeleteObject(string key);

        void DeleteObject(string group, string key);

        IEnumerable<JustineLanguage> GetLanguages();
    }
}
