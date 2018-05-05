using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace JustineCore.Discord.Modules
{
    public class Basics : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task Greet()
        {
            await ReplyAsync($"Hey, {Context.User.Mention}!");
        }
    }
}
