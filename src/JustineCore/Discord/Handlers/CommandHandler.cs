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
using System.Text.RegularExpressions;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Discord.Features.TutorialServer;

namespace JustineCore.Discord.Handlers
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private IServiceProvider _services;
        private ILocalization _lang;
        private WaitingRoomService _waitingRoomService;
        private ProblemBoardService _problemBoardService;

        private VerificationProvider _botVer;

        internal async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            Global.Client = client;
            _commandService = new CommandService();

            _lang = Unity.Resolve<ILocalization>();

            _botVer = Unity.Resolve<VerificationProvider>();

            _waitingRoomService = Unity.Resolve<WaitingRoomService>();

            _problemBoardService = Unity.Resolve<ProblemBoardService>();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_lang)
                .AddSingleton(Unity.Resolve<IDataStorage>())
                .AddSingleton(Unity.Resolve<GlobalUserDataProvider>())
                .AddSingleton(Unity.Resolve<AppConfig>())
                .AddSingleton(Unity.Resolve<RpgRepository>())
                .AddSingleton(Unity.Resolve<DiggingJobProvider>())
                .AddSingleton(_waitingRoomService)
                .AddSingleton(_problemBoardService)
                .AddSingleton(Unity.Resolve<ProblemProvider>())
                .AddSingleton(_botVer)
                .BuildServiceProvider();

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
            _client.MessageReceived += _waitingRoomService.MessageReceived;
            _client.UserJoined += _waitingRoomService.UserJoined;
            _client.UserLeft += _waitingRoomService.UserLeft;

            _client.MessageReceived += _problemBoardService.MessageReceived;

            _client.ReactionAdded += DmDeletions.CheckDeletionRequest;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Channel is SocketDMChannel) return;

            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) 
            {
                ShowcaseVerificationGate(context);
                CheckForBotValidation(context);
                return;
            }

            var argPos = 0;
#if DEBUG
            // return if not DEBUG guild
            //if(context.Guild.Id != Constants.ServiceServerId) return;
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

        private async void ShowcaseVerificationGate(SocketCommandContext context)
        {
            if(context.User.Id == Constants.JustineId) return;

            if(context.Channel.Id != Constants.TutorialBotShowcaseChannelId) return;

            if(!context.User.IsBot) return;

            if(_botVer.IsVerified(context.User.Id)) return;

            await context.Message.DeleteAsync();

            await context.Channel.SendMessageAsync($"The {context.User.Mention} <:bot:400105688967413781> is not verified. Only verified bots can respond in this channel.");
        }

        private async void CheckForBotValidation(SocketCommandContext context)
        {
            if(context.Message.MentionedUsers.Count != 1) return;

            if(context.Message.Content.Contains("[NOT-VALIDATION]")) return;

            var mentioned = context.Message.MentionedUsers.FirstOrDefault();

            if(mentioned == null) return;

            if(mentioned.IsBot) return;

            var success = _botVer.CheckVerification(mentioned.Id, context.User.Id, context.Message.Content);

            if(!success) return;
            
            var role = context.Guild.Roles.FirstOrDefault(r => r.Id == 381409798903824394);

            if(!(role is null))
            {
                await ((SocketGuildUser)mentioned).AddRoleAsync(role);
            }

            await context.Channel.SendMessageAsync($"{context.User.Mention} is now verified as a <:bot:400105688967413781> created by {mentioned.Mention}.");
        }

        private async Task TryRunAsBotCommand(SocketCommandContext context, int argPos)
        {
            var cmdSearchResult = _commandService.Search(context, argPos);
            if (cmdSearchResult.Commands.Count == 0)
            {
                CheckIfDoubleSpace(context);
                return;
            }

            Logger.Log($"[Command] {context.User.Username} is running '{cmdSearchResult.Commands.FirstOrDefault().Command.Name}' - Full message: '{context.Message.Content}'");

            var commandTask = _commandService.ExecuteAsync(context, argPos, _services);

            #pragma warning disable CS4014
            commandTask.ContinueWith(task => 
            {
                if (!task.Result.IsSuccess)
                {
                    var exceptionMessage = _lang.FromTemplate("EXCEPTION_RESPONSE_TEMPLATE(@REASON)", objects: task.Result.ErrorReason);
                    context.Channel.SendMessageAsync(exceptionMessage);
                }
            });
            #pragma warning restore CS4014
        }

        private async void CheckIfDoubleSpace(SocketCommandContext context)
        {
            var regex = new Regex("  +");
            if(!regex.IsMatch(context.Message.Content)) return;

            await context.Channel.SendMessageAsync(_lang.Resolve($"{context.User.Mention}, [COMMAND_ERROR_DOUBLE_SPACES]"));
        }
    }
}