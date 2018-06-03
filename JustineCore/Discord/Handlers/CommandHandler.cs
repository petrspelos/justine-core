using Discord.Commands;
using Discord.WebSocket;
using JustineCore.Discord.Features;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;
using JustineCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JustineCore.Configuration;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Features.RPG.GoldDigging;

namespace JustineCore.Discord.Handlers
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _services;
        private ILocalization _lang;

        internal async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _commandService = new CommandService();

            _lang = Unity.Resolve<ILocalization>();

            _services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(_commandService)
                .AddSingleton(_lang)
                .AddSingleton(Unity.Resolve<IDataStorage>())
                .AddSingleton(Unity.Resolve<GlobalUserDataProvider>())
                .AddSingleton(Unity.Resolve<AppConfig>())
                .AddSingleton(Unity.Resolve<RpgRepository>())
                .AddSingleton(Unity.Resolve<DiggingJobProvider>())
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
#if DEBUG
            // return if not DEBUG guild
            if(context.Guild.Id != 338393057437286411) return;
            if (msg.HasStringPrefix("> ", ref argPos))
            {
                await TryRunAsBotCommand(context, argPos);
            }
#else
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                await TryRunAsBotCommand(context, argPos);
            }
#endif
        }

        private async Task TryRunAsBotCommand(SocketCommandContext context, int argPos)
        {
            var cmdSearchResult = _commandService.Search(context, argPos);
            if (cmdSearchResult.Commands.Count == 0) return;

            Logger.Log($"[Command] {context.User.Username} is running '{cmdSearchResult.Commands.FirstOrDefault().Command.Name}' - Full message: '{context.Message.Content}'");

            var commandTask = _commandService.ExecuteAsync(context, argPos, _services);

            #pragma warning disable CS4014
            commandTask.ContinueWith(task => 
            {
                if (!task.Result.IsSuccess && task.Result.Error != CommandError.UnknownCommand)
                {
                    var exceptionMessage = _lang.FromTemplate("EXCEPTION_RESPONSE_TEMPLATE(@REASON)", objects: task.Result.ErrorReason);
                    context.Channel.SendMessageAsync(exceptionMessage);
                }
            });
            #pragma warning restore CS4014
        }
    }
}