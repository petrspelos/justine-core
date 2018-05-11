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

        [Command("log clear")]
        [RequireOwner]
        public async Task ClearLog()
        {
            await ReplyAsync("The ClearLog request has been sent.");
            Logger.ClearLog();
            Logger.Log($"[Logger] The runtime.log has been cleared by '{Context.User.Username}' ({Context.User.Id})");
        }

        [Command("log get")]
        [RequireOwner]
        public async Task GetLog()
        {
            Logger.Log($"[Logger] The runtime.log is being sent to '{Context.User.Username}' ({Context.User.Id})");
            await Context.Channel.SendFileAsync("runtime.log");
        }
    }
}
