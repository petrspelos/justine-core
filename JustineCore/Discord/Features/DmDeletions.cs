using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace JustineCore.Discord.Features
{
    public class DmDeletions
    {
        private const string DeletionRequestEmoji = "❌";

        public static async Task CheckDeletionRequest(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!(channel is SocketDMChannel)) return;
            if (reaction.Emote.Name != DeletionRequestEmoji) return;

            var msg = await channel.GetMessageAsync(reaction.MessageId);
            await msg.DeleteAsync();
        }
    }
}
