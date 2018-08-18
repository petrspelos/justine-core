using Discord;
using Discord.Net;
using Discord.WebSocket;
using JustineCore.Configuration;
using JustineCore.Discord.Features.Payloads;
using JustineCore.Discord.Handlers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;
using System;
using JustineCore.Discord.Features.RPG.GoldDigging;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Discord.Features.TutorialServer;
using static JustineCore.SchedulerUtilities;

namespace JustineCore.Discord
{
    public class Connection
    {
        //internal DiscordSocketClient client;
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;
        private readonly AppConfig _appConfig;
        private readonly ProblemBoardService _problemBoardService;

        public Connection(AppConfig config, DiscordSocketClient client, ProblemBoardService problemBoardService, CommandHandler commandHandler)
        {
            _appConfig = config;
            _client = client;
            _problemBoardService = problemBoardService;
            _commandHandler = commandHandler;
        }

        internal async Task NotifyOwner(string message)
        {
            if(_client.ConnectionState != ConnectionState.Connected) return;
            var owner = _client.GetUser(182941761801420802);
            var dm = await owner.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync(message);
        }
        
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            ValidateToken();

            _client.Log += Logger.Log;
            _client.Ready += OnReady;

            await _commandHandler.InitializeAsync(_client);

            try
            {
                await _client.LoginAsync(TokenType.Bot, _appConfig.DiscordBotConfig.Token);
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Unauthorized)
                {
                    throw new InvalidTokenException("Unauthorized");
                }
            }

            await _client.StartAsync();

            RegisterScheduledMessages();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task OnReady()
        {
            var djp = Unity.Resolve<DiggingJobProvider>();
            djp.RegisterAllJobs();

            await RegisterScheduledProblemCleanup();

            try
            {
                await _client.GetUser(Constants.PeterId).SendMessageAsync("I'm online.");
            }
            catch
            {
                Logger.Log("Could not send a status DM.");
            }
        }

        private void ValidateToken()
        {
            if (_appConfig.DiscordBotConfig.Token == null)
                throw new TokenNotSetException("Discord bot token is null.");
        }

        private void RegisterScheduledMessages()
        {
            foreach (var sm in _appConfig.DiscordBotConfig.ScheduledMessages)
            {
                ExecuteEveryDayAt(() => { ExecuteScheduledMessage(sm); }, sm.Hour, sm.Minute);
            }
        }

        private async Task RegisterScheduledProblemCleanup()
        {
            await _problemBoardService.RoutineProblemCleanup();
            ExecuteEverHours(() => { _problemBoardService.RoutineProblemCleanup(); }, 1);
        }

        private void ExecuteScheduledMessage(ScheduledMessage sm)
        {
            foreach (var p in sm.Payloads)
            {
                p.Send(_client);
            }
        }

        internal void ReloadAllScheduledTasks()
        {
            JobManager.RemoveAllJobs();
            RegisterScheduledMessages();
        }
    }
}
