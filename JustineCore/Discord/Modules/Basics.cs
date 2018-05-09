using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Preconditions;

namespace JustineCore.Discord.Modules
{
    public class Basics : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task Greet()
        {
            await ReplyAsync($"Hey, {Context.User.Mention}!");
        }

        [Command("dataCommand")]
        [RequireDataCollectionConsent]
        public async Task DataCommand()
        {
            await ReplyAsync("Hey, you gave your consent!");
        }
    }
}
