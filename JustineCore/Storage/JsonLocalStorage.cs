using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustineCore.Entities;
using Newtonsoft.Json;
using JustineCore.Storage.Helpers;

namespace JustineCore.Storage
{
    public class JsonLocalStorage : IDataStorage
    {
        private const string StorageDirectory = "jsonStorage";
        private const string LanguagesDirectory = "Language/Data";

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

        public void StoreObject(object obj, string group, string key)
        {
            var targetDirectory = $"{StorageDirectory}/{group}";
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);
            StoreObject(obj, $"{group}/{key}");
        }

        public T RestoreObject<T>(string group, string key)
        {
            if(!Directory.Exists($"{StorageDirectory}/{group}"))
                throw new DataStorageGroupDoesNotExistException($"Group '{group}' not found.");

            return RestoreObject<T>($"{group}/{key}");
        }

        public IEnumerable<T> RestoreGroup<T>(string group)
        {
            var files = new string[0];
            try
            {
                files = Directory.GetFiles($"{StorageDirectory}/{group}", "*.json");
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory($"{StorageDirectory}/{group}");
            }

            try
            {
                return files.Select(Json.ObjectFromJsonFile<T>);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public void DeleteObject(string key)
        {
            File.Delete(GetJsonFilePathFromKey(key));
        }
        
        public void DeleteObject(string group, string key)
        {
            DeleteObject($"{group}/{key}");
        }

        public IEnumerable<JustineLanguage> GetLanguages()
        {
            var langFiles = Directory.GetFiles(LanguagesDirectory, "*.json");
            return langFiles.Select(f => f.ParseAsJsonFilePath<JustineLanguage>());
        }
    }
}
