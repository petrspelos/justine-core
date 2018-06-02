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
using static JustineCore.Utility;
using System;
using JustineCore.Discord.Features.RPG.GoldDigging;

namespace JustineCore.Discord
{
    public class Connection
    {
        internal DiscordSocketClient client;
        private CommandHandler _commandHandler;
        private readonly AppConfig _appConfig;

        public Connection(AppConfig config)
        {
            _appConfig = config;
        }

        internal async Task NotifyOwner(string message)
        {
            if(client.ConnectionState != ConnectionState.Connected) return;
            var owner = client.GetUser(182941761801420802);
            var dm = await owner.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync(message);
        }
        
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            ValidateToken();

            var socketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            };

            client = new DiscordSocketClient(socketConfig);
            client.Log += Logger.Log;
            client.Ready += OnReady;

            _commandHandler = new CommandHandler();
            await _commandHandler.InitializeAsync(client);

            try
            {
                await client.LoginAsync(TokenType.Bot, _appConfig.DiscordBotConfig.Token);
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Unauthorized)
                {
                    throw new InvalidTokenException("Unauthorized");
                }
            }

            await client.StartAsync();

            RegisterScheduledMessages();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private Task OnReady()
        {
            var djp = Unity.Resolve<DiggingJobProvider>();
            djp.RegisterAllJobs();

            return Task.CompletedTask;
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

        private void ExecuteScheduledMessage(ScheduledMessage sm)
        {
            foreach (var p in sm.Payloads)
            {
                p.Send(client);
            }
        }

        internal void ReloadAllScheduledTasks()
        {
            JobManager.RemoveAllJobs();
            RegisterScheduledMessages();
        }
    }
}
