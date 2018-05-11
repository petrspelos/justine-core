using System;
using JustineCore.Entities;
using JustineCore.Storage;
using System.Linq;
using JustineCore.Discord;

namespace JustineCore.Configuration
{
    public class AppConfig
    {
        internal DiscordBotConfig DiscordBotConfig;

        private readonly IDataStorage _storage;
        private const string BotConfigKey = "botConfig";

        public AppConfig(IDataStorage storage)
        {
            _storage = storage;
            LoadStoredBotConfig();
        }

        /// <summary>
        /// Loads stored configuration settings for the Discord bot. Creates a new one if none found.
        /// </summary>
        internal void LoadStoredBotConfig()
        {
            try
            {
                DiscordBotConfig = _storage.Get<DiscordBotConfig>(BotConfigKey);
                Logger.Log("[Configuration] Loaded botConfig.json.");
            }
            catch (DataStorageKeyDoesNotExistException)
            {
                Logger.Log("[Configuration] No botConfig.json found. Defaulting to a new config.");
                DiscordBotConfig = new DiscordBotConfig();
                StoreCurrentBotConfig();
            }
        }

        internal void ApplyArguments(string[] args)
        {
            var passedToken = args
                .Where(a => a.StartsWith("-t:"))
                .Select(a => a.Substring(3))
                .FirstOrDefault();

            if (passedToken is null) return;
            DiscordBotConfig.Token = passedToken;
            if (args.Contains("-f")) StoreCurrentBotConfig();
        }

        private void StoreCurrentBotConfig()
        {
            _storage.Store(DiscordBotConfig, BotConfigKey);
            Logger.Log("[Configuration] Saved current botConfig.json for future boots.");
        }
    }
}
