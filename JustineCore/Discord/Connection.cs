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

namespace JustineCore.Discord
{
    internal class Connection
    {
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;

        internal async Task NotifyOwner(string message)
        {
            if(_client.ConnectionState != ConnectionState.Connected) return;
            var owner = _client.GetUser(182941761801420802);
            var dm = await owner.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync(message);
        }
        
        public async Task ConnectAsync(AppConfig appConfig, CancellationToken cancellationToken)
        {
            if (AppConfig.DiscordBotConfig.Token == null)
                throw new TokenNotSetException("Discord bot token is null.");

            var socketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            };

            _client = new DiscordSocketClient(socketConfig);
            _client.Log += Logger.Log;

            _commandHandler = new CommandHandler();
            await _commandHandler.InitializeAsync(_client);

            try
            {
                await _client.LoginAsync(TokenType.Bot, AppConfig.DiscordBotConfig.Token);
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

        private void RegisterScheduledMessages()
        {
            foreach (var sm in AppConfig.DiscordBotConfig.ScheduledMessages)
            {
                ExecuteEveryDayAt(() => { ExecuteScheduledMessage(sm); }, sm.Hour, sm.Minute);
            }
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
