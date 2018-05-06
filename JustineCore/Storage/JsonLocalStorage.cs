using System;
using System.IO;
using Newtonsoft.Json;

namespace JustineCore.Storage
{
    public class JsonLocalStorage : IDataStorage
    {
        private const string StorageDirectory = "jsonStorage";

        public JsonLocalStorage()
        {
            if (!Directory.Exists(StorageDirectory))
            {
                Directory.CreateDirectory(StorageDirectory);
            }
        }
        
        public void StoreObject(object obj, string key)
        {
            var json = JsonConvert.SerializeObject(obj);
            var filePath = GetJsonFilePathFromKey(key);

            File.WriteAllText(filePath, json);
        }

        public T RestoreObject<T>(string key)
        {
            var filePath = GetJsonFilePathFromKey(key);

            try
            {
                var jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (FileNotFoundException)
            {
                throw new DataStorageKeyDoesNotExistException(
                    $"No object stored with the '{key}' key."
                );
            }
        }

        private static string GetJsonFilePathFromKey(string file)
        {
            return $"{StorageDirectory}/{file}.json";
        }
    }

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
}
