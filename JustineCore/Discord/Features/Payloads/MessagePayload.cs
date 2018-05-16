using Discord.WebSocket;

namespace JustineCore.Discord.Features.Payloads
{
    public class MessagePayload
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string MessageTemplate { get; set; }

        public async void Send(DiscordSocketClient client)
        {
            var guild = client.GetGuild(GuildId);
            var textChannel = guild.GetTextChannel(ChannelId);

            var msg = Utility.ResolvePlaceholders(MessageTemplate);
            await textChannel.SendMessageAsync(msg);
        }
    }
}