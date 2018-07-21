using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using JustineCore.Configuration;
using JustineCore.Discord.Features.Payloads;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Discord.Providers.UserData;
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
    public class Debug : ModuleBase<SocketCommandContext>
    {
        private GlobalUserDataProvider _gudp;
        private VerificationProvider _botVer;
        private DiscordSocketClient _client;

        public Debug(GlobalUserDataProvider gudp, VerificationProvider botVer, DiscordSocketClient client)
        {
            _gudp = gudp;
            _botVer = botVer;
            _client = client;
        }

        [Command("OnAdventure")]
        [RequireOwner]
        public async Task DebugCmd(IGuildUser target)
        {
            var tName = target.Nickname??target.Username;
            
            if(!_gudp.GlobalDataExists(target.Id))
            {
                await ReplyAsync($"{tName} - No consent");
                return;
            }

            var onAdventure = _gudp.GetGlobalUserData(target.Id).RpgAccount.OnAdventure;

            await ReplyAsync($"{tName} - OnAdventure: {onAdventure}");
        }

        [Command("dbg")]
        [RequireOwner]
        public async Task TestingS()
        {
            try
            {
                var path = Utility.GetTestImage(Context.User.GetAvatarUrl());
                await Context.Channel.SendFileAsync(path);
            }
            catch(Exception e)
            {
                var log = $"[Stringified Exception] {e}\n[Stack Trace] {e.StackTrace}";
                Discord.Logger.Log(log);
                await Context.Channel.SendMessageAsync(log);
            }
        }

        [Command("dbg-verify")]
        [RequireOwner]
        public async Task Verify()
        {
            if(_botVer.IsVerified(_client.CurrentUser.Id)) return;

            var verification = _botVer.SearchByPredicate(d => d.BotId == _client.CurrentUser.Id && d.OwnerId == 182941761801420802).FirstOrDefault();

            if(verification == null) return;

            await ReplyAsync($"<@!182941761801420802> {verification.VerificationString}");
        }

        [Command("dbg-clear-verification")]
        [RequireOwner]
        public async Task ClearVerification(IGuildUser bot)
        {
            if(!bot.IsBot) return;

            if(!_botVer.IsVerified(bot.Id)) return;

            _botVer.ClearValidation(bot.Id);

            await ReplyAsync($"Validation of {bot.Mention} has been reset.");
        }

        [Command("dbg-verify-as")]
        [RequireOwner]
        public async Task FakeVerify(IGuildUser bot, IGuildUser fake)
        {
            var a = _botVer.CreateNewVerification(bot.Id, fake.Id);
            if(a == null) return;
            await ReplyAsync(a);
        }
    }
}
