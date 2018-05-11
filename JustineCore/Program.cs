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
        private static async Task Main(string[] args)
        {
            Unity.RegisterTypes();

            var appConfig = Unity.Resolve<AppConfig>();
            appConfig.ApplyArguments(args);
            
            var discordConnection = new Discord.Connection();

            await discordConnection.ConnectAsync(appConfig.DiscordBotConfig, CancellationToken.None);
        }
    }
}
