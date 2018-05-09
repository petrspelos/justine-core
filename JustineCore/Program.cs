using JustineCore.Configuration;
using JustineCore.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 4014

namespace JustineCore
{
    internal class Program
    {
        private static void Main(string[] args) => StartAsync(args).GetAwaiter().GetResult();

        private static async Task StartAsync(string[] args)
        {
            Unity.RegisterTypes();

            var appConfig = Unity.Resolve<AppConfig>();
            var botConfigDefault = new DiscordBotConfig();

            var passedToken = args.Where(a => a.StartsWith("-t:")).Select(a => a.Substring(3)).FirstOrDefault();
            if (passedToken != null) botConfigDefault.Token = passedToken;

            appConfig.LoadStoredBotConfig(botConfigDefault);

            if (passedToken != null && args.Contains("-f")) appConfig.DiscordBotConfig.Token = passedToken;

            var discordConnection = new Discord.Connection();

            await discordConnection.ConnectAsync(appConfig.DiscordBotConfig, CancellationToken.None);
        }
    }
}
