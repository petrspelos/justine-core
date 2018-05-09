using System;
using System.ComponentModel.Design;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Features;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;
using JustineCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace JustineCore.Discord.Handlers
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _services;

        internal async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _commandService = new CommandService();

            var localization = Unity.Resolve<ILocalization>();
            var dataStorage = Unity.Resolve<IDataStorage>();
            var globalUserDataProvider = Unity.Resolve<GlobalUserDataProvider>();

            _services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(_commandService)
                .AddSingleton(localization)
                .AddSingleton(dataStorage)
                .AddSingleton(globalUserDataProvider)
                .BuildServiceProvider();

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;

            _client.ReactionAdded += DmDeletions.CheckDeletionRequest;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Channel is SocketDMChannel) return;

            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;
            
            var argPos = 0;
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos) || msg.HasStringPrefix("dbg ", ref argPos))
            {
                await TryRunAsBotCommand(context, argPos);
            }
        }

        private async Task TryRunAsBotCommand(SocketCommandContext context, int argPos)
        {
            var cmdSearchResult = _commandService.Search(context, argPos);
            if (cmdSearchResult.Commands.Count == 0) return;

            var result = await _commandService.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync($":anger: **Couldn't complete the operation**\nApparently, {result.ErrorReason}");
            }
        }
    }
}