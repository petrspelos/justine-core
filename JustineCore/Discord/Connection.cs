﻿using Discord;
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
        //internal DiscordSocketClient client;
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;
        private readonly AppConfig _appConfig;

        public Connection(AppConfig config, DiscordSocketClient client)
        {
            _appConfig = config;
            _client = client;
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

            var socketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            };

            _client.Log += Logger.Log;
            _client.Ready += OnReady;
            //client.UserVoiceStateUpdated += VoiceStateUpdated;

            _commandHandler = new CommandHandler();
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

            await NotifyOwner("Justine Online");
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
