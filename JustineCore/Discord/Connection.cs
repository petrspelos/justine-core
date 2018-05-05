using System.Net;
using System.Threading;
using Discord.WebSocket;
using JustineCore.Entities;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using JustineCore.Discord.Handlers;

namespace JustineCore.Discord
{
    internal class Connection
    {
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;

        public async Task ConnectAsync(DiscordBotConfig botConfig, CancellationToken cancellationToken)
        {
            if(botConfig.Token == null)
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
                await _client.LoginAsync(TokenType.Bot, botConfig.Token);
            }
            catch (HttpException e)
            {
                if (e.HttpCode == HttpStatusCode.Unauthorized)
                {
                    throw new InvalidTokenException("Unauthorized");
                }
            }

            await _client.StartAsync();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
