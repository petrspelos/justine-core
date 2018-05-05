using JustineCore.Entities;
using JustineCore.Storage;

namespace JustineCore.Configuration
{
    internal class AppConfig
    {
        internal DiscordBotConfig DiscordBotConfig;

        private readonly IDataStorage _dataStorage;
        private const string BotConfigKey = "botConfig";

        internal AppConfig(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        /// <summary>
        /// Returns stored config. If none is found, creates a new one, saves and returns it.
        /// </summary>
        /// <param name="backup">Instance of DiscordBotConfig to be used instead of new DiscordBotConfig();</param>
        /// <returns>Stored or newly stored DiscordBotConfig</returns>
        internal void LoadStoredBotConfig(DiscordBotConfig backup = null)
        {
            try
            {
                DiscordBotConfig = _dataStorage.RestoreObject<DiscordBotConfig>(BotConfigKey);
            }
            catch (DataStorageKeyDoesNotExistException)
            {
                DiscordBotConfig = backup ?? new DiscordBotConfig();
                OverwriteBotConfig(DiscordBotConfig);
            }
        }

        private void OverwriteBotConfig(DiscordBotConfig newConfig)
        {
            _dataStorage.StoreObject(newConfig, BotConfigKey);
        }
    }
}
