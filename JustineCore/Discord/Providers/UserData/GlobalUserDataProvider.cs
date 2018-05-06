using System;
using System.Collections.Generic;
using System.Linq;
using JustineCore.Entities;
using JustineCore.Storage;

namespace JustineCore.Discord.Providers.UserData
{
    public class GlobalUserDataProvider
    {
        private const string GlobalDataGroup = "GlobalUserData";
        private const string GlobalDataKeyFormat = "gd{0}";

        private readonly IDataStorage _dataStorage;
        private List<GlobalUserData> _globalUserDatas;

        public GlobalUserDataProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            LoadGlobalUserData();
        }

        public void AddNewGlobalData(ulong userId, DataConsent consent)
        {
            if (GlobalDataExists(userId)) return;

            var newUserData = new GlobalUserData
            {
                DiscordId = userId,
                CollectionConsent = consent
            };

            var key = string.Format(GlobalDataKeyFormat, userId);
            _dataStorage.StoreObject(newUserData, GlobalDataGroup, key);

            _globalUserDatas.Add(newUserData);
        }

        public void DeleteUserGlobalData(ulong userId)
        {
            if (!GlobalDataExists(userId)) return;

            var key = string.Format(GlobalDataKeyFormat, userId);

            try
            {
                _globalUserDatas.Remove(GetGlobalUserData(userId));
                _dataStorage.DeleteObject(GlobalDataGroup, key);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Storage Exception: {e.Message}");
            }
        }

        public bool GlobalDataExists(ulong userId)
        {
            return _globalUserDatas.Any(d => d.DiscordId == userId);
        }

        public GlobalUserData GetGlobalUserData(ulong userId)
        {
            return _globalUserDatas.FirstOrDefault(d => d.DiscordId == userId);
        }

        private void LoadGlobalUserData()
        {
            _globalUserDatas = _dataStorage.RestoreGroup<GlobalUserData>(GlobalDataGroup).ToList();
        }
    }
}