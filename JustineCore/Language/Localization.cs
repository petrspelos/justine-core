using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustineCore.Entities;
using Newtonsoft.Json;

namespace JustineCore.Language
{
    public static class Localization
    {
        private const string LanguageFilesDirectory = "Language/Data";
        private static readonly List<JustineLanguage> Languages = new List<JustineLanguage>();

        static Localization()
        {
            LoadJsonLanguageFiles();
        }

        /// <summary>
        /// Returns a string value of a language resource by the resource key and language ID.
        /// </summary>
        /// <param name="key">Resource's key.</param>
        /// <param name="languageId">Language ID defined in its .json file.</param>
        /// <exception cref="LanguageNotFoundException"></exception>
        /// <exception cref="LanguageResourceKeyNotFoundException"></exception>
        /// <returns>value of a language resource</returns>
        public static string GetResource(string key, uint languageId = 0)
        {
            var language = Languages.FirstOrDefault(l => l.LanguageId == languageId);
            if (language is null) throw new LanguageNotFoundException($"Id '{languageId}' not found.");
            if (!language.Resources.ContainsKey(key)) throw new LanguageResourceKeyNotFoundException($"Resource with key '{key}' wasn't found.");
            return language.Resources[key];
        }

        private static void LoadJsonLanguageFiles()
        {
            
            var langFiles = Directory.GetFiles(LanguageFilesDirectory, "*.json");
            foreach (var filePath in langFiles)
            {
                var success = TryParseLanguageFile(filePath);
                if(!success) Console.WriteLine($"Failed to load language '{filePath}'");
            }
        }

        private static bool TryParseLanguageFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                Languages.Add(JsonConvert.DeserializeObject<JustineLanguage>(json));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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