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
        
        public bool GlobalDataExists(ulong userId)
        {
            return _globalUserDatas.Any(d => d.DiscordId == userId);
        }

        private void LoadGlobalUserData()
        {
            _globalUserDatas = _dataStorage.RestoreGroup<GlobalUserData>(GlobalDataGroup).ToList();
        }
    }
}