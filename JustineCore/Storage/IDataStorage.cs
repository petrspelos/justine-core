using System.Collections.Generic;

namespace JustineCore.Storage
{
    public interface IDataStorage
    {
        void StoreObject(object obj, string key);

        void StoreObject(object obj, string group, string key);

        T RestoreObject<T>(string key);

        T RestoreObject<T>(string group, string key);

        IEnumerable<T> RestoreGroup<T>(string group);
    }
}
