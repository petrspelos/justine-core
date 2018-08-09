using System;
using System.Collections.Generic;
using System.Linq;
using JustineCore.Entities;
using JustineCore.Storage;
using JustineCore.Discord.Features.RPG.Actions;
using System.Collections.Concurrent;

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
            _dataStorage.Store(newUserData, GlobalDataGroup, key);

            _globalUserDatas.Add(newUserData);
        }

        public void DeleteUserGlobalData(ulong userId)
        {
            if (!GlobalDataExists(userId)) return;

            var key = string.Format(GlobalDataKeyFormat, userId);
            
            _globalUserDatas.Remove(GetGlobalUserData(userId));
            _dataStorage.Delete(GlobalDataGroup, key);
        }

        public bool GlobalDataExists(ulong userId)
        {
            return _globalUserDatas.Any(d => d.DiscordId == userId);
        }

        public GlobalUserData GetGlobalUserData(ulong userId)
        {
            return _globalUserDatas.FirstOrDefault(d => d.DiscordId == userId);
        }

        public IEnumerable<GlobalUserData> SearchByPredicate(Func<GlobalUserData, bool> predicate)
        {
            return _globalUserDatas.Where(predicate);
        }

        public void SaveGlobalUserData(GlobalUserData data)
        {
            var key = string.Format(GlobalDataKeyFormat, data.DiscordId);
            _dataStorage.Store(data, GlobalDataGroup, key);
        }

        private void LoadGlobalUserData()
        {
            // TODO: FIXME: get a dictionary from a group collection by getting individual Key-Value pairs
            _globalUserDatas = _dataStorage.GetGroup<GlobalUserData>(GlobalDataGroup).ToList();
        }
    }
}