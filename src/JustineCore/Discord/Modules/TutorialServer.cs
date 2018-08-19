using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JustineCore.Configuration;
using JustineCore.Discord.Features.Payloads;
using JustineCore.Discord.Features.TutorialServer;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JustineCore.Discord.Modules
{
    [RequireGuildById(Constants.TutorialServerId)]
    public class TutorialServer : ModuleBase<SocketCommandContext>
    {
        private readonly GlobalUserDataProvider _gudp;
        private readonly VerificationProvider _botVer;
        private readonly ProblemBoardService _pbService;
        private readonly ProblemProvider _problemProvider;
        private readonly ILocalization _lang;

        public TutorialServer(GlobalUserDataProvider gudp, VerificationProvider botVer, ProblemBoardService pbService, ProblemProvider problemProvider, ILocalization lang)
        {
            _gudp = gudp;
            _botVer = botVer;
            _pbService = pbService;
            _problemProvider = problemProvider;
            _lang = lang;
        }

        [Command("redeploy")]
        [RequireOwner]
        public async Task RedeployingNotification()
        {
            var general = Context.Guild.GetTextChannel(Constants.TutorialGeneralId);
            await general.SendMessageAsync(_lang.GetPooledResource("REDEPLOYING_NOTIFICATION"));
        }

        [Command("redeploy done")]
        [RequireOwner]
        public async Task DoneRedeployingNotification()
        {
            var general = Context.Guild.GetTextChannel(Constants.TutorialGeneralId);
            await general.SendMessageAsync(_lang.GetPooledResource("REDEPLOYING_NOTIFICATION_DONE"));
        }

        [Command("solve")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminSolveProblem(IGuildUser user, int problemId)
        {
            try
            {
                await _pbService.SolveProblemForUser(problemId, user.Id);
                await ReplyAsync("Done");
            }
            catch(Exception)
            {
                var acc = _problemProvider.GetAccount(user.Id);
                await ReplyAsync($"The Problem ID is most likely invalid. This might help you with debugging:\n\n```\nAccount has {acc.Problems.Count} active problems.\n```");
            }
        }

        [Command("problem for")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminCreateProblem(IGuildUser user, [Remainder]string problem)
        {
            await _pbService.CreateProblemForUser(problem, user.Id);
            await ReplyAsync("Done");
        }

        [Command("unverified-bots")]
        [RequireOwner]
        public async Task GetUnverifiedBots()
        {
            var bots = Context.Guild.Users.Where(u => u.IsBot && !_botVer.IsVerified(u.Id)).Select(b => b.Mention);

            if(bots.Count() == 0)
            {
                await ReplyAsync("There are no unverified bots on this server.");
                return;
            }

            try
            {
                await ReplyAsync($"Theses are the unverified bots of this server:\n{string.Join("\n", bots)}");
            }
            catch(Exception)
            {
                await ReplyAsync("There are too many unverified bots to list on this server.");
            }
        }

        [Command("bots"), Alias("bot")]
        public async Task GetBots(IGuildUser target)
        {
            if(target.IsBot)
            {
                await ReplyAsync($"{target.Mention}, BOTs don't have BOTs. Although that would be wild... And I should probably make one. :thinking:");
                return;
            }

            var ownedBots = _botVer.SearchByPredicate(d => d.OwnerId == target.Id && d.Verified == true).ToList();

            if(!ownedBots.Any())
            {
                await ReplyAsync($"{target.Mention}, {target.Nickname??target.Username} does not have any registered BOTs.");
                return;
            }

            var result = new StringBuilder();
            result.Append($"{target.Mention}, {target.Nickname??target.Username} owns the following bots:\n");
            foreach(var vBot in ownedBots)
            {
                var bot = Context.Guild.GetUser(vBot.BotId);
                if(bot is null) continue;
                result.Append($"{bot.Mention}\n");
            }
            result.Append($"_If this list is empty, please notify Peter, because it means the BOT/s got kicked without proper closure._");

            await ReplyAsync(result.ToString());
        }

        [Command("owner")]
        public async Task GetOwner(IGuildUser target)
        {
            if(!target.IsBot) 
            {
                await ReplyAsync($"Turns out {target.Nickname??target.Username} is a human being. Owning a human being is called **slavery**.");
                return;
            }

            if(!_botVer.IsVerified(target.Id))
            {
                await ReplyAsync($"{target.Mention} is not a verified bot. (Unverified bots will be kicked after a period of time.)");
                return;
            }

            var result = _botVer.SearchByPredicate(d => d.BotId == target.Id && d.Verified == true).FirstOrDefault();

            if(result is null)
            {
                await ReplyAsync($"{target.Mention} is not a verified bot. (Unverified bots will be kicked after a period of time.)\n\nNow don't mind me as I call for help... <@182941761801420802>");
                return;
            }

            var user = Context.Guild.GetUser(result.OwnerId);

            if(user is null)
            {
                await ReplyAsync($"The author of {target.Mention} is no longer in this server. (or there is a bug in this search, but what are the chances of that happening... again...?)");
                return;
            }

            if(user.Id == Context.User.Id)
            {
                await ReplyAsync($"Oh boy, would you look at that... You are the owner of {target.Mention}, {Context.User.Mention}");
                return;
            }

            await ReplyAsync($"The owner of {target.Mention} is {user.Mention}.");
        }

        [Command("verify")]
        public async Task DebugCmd(IGuildUser bot)
        {
            if(!bot.IsBot)
            {
                await ReplyAsync($"Very funny, {Context.User.Mention}. You can only verify bots.");
                return;
            }

            if(_botVer.IsVerified(bot.Id))
            {
                await ReplyAsync($"{bot.Mention} is already verified.");
                return;
            }

            if(_botVer.BotVerificationExists(bot.Id, Context.User.Id))
            {
                var verification = _botVer.SearchByPredicate(d => d.BotId == bot.Id && d.OwnerId == Context.User.Id && d.Verified == false).FirstOrDefault();
                
                if(verification == null)
                {
                    await ReplyAsync($"Something went wrong with your verification. Please contact <@!182941761801420802>.");
                    return;
                }

                await ReplyAsync($@"[NOT-VALIDATION]
{Context.User.Mention},
to verify your bot, make it send a message that **mentions you** and includes the **following code**:
`{verification.VerificationString}`

If you have a feature that would allow others to validate your bot instead of you, include [NOT-VALIDATION] in such message.");
                return;
            }

            var code = _botVer.CreateNewVerification(bot.Id, Context.User.Id);

            if(code == null)
            {
                await ReplyAsync($"Something went wrong with your verification. Please contact <@!182941761801420802>.");
                return;
            }

            await ReplyAsync($@"[NOT-VALIDATION]
{Context.User.Mention},
to verify your bot, make it send a message that **mentions you** and includes the **following code**:
`{code}`

If you have a feature that would allow others to validate your bot instead of you, include [NOT-VALIDATION] in such message.");
        }
    }
}
