using Discord;
using Discord.Commands;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
using JustineCore.Language;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace JustineCore.Discord.Modules
{
    public class Privacy : ModuleBase<SocketCommandContext>
    {
        private readonly ILocalization _lang;
        private readonly GlobalUserDataProvider _gudp;

        public Privacy(ILocalization lang, GlobalUserDataProvider gudp)
        {
            _lang = lang;
            _gudp = gudp;
        }

        [Command("I agree with my data being collected")]
        [Alias("I agree with my data being collected.")]
        [Summary("SUMMARY_DATA_ACCEPT")]
        public async Task GiveConsent()
        {
            var userId = Context.User.Id;

            if (_gudp.GlobalDataExists(userId)) return;

            var consent = new DataConsent
            {
                Date = DateTime.UtcNow,
                MessageId = Context.Message.Id
            };

            _gudp.AddNewGlobalData(userId, consent);
            
            await ReplyAsync(_lang.Resolve("[PRIVACY_MESSAGE_HEADER]\n\n[PRIVACY_AGREE_RESPONSE]\n\n[PRIVACY_CMD_HINT]"));
        }

        [Command("data-view")]
        [Alias("data-show")]
        [Summary("SUMMARY_DATA_VIEW")]
        [RequireDataCollectionConsent]
        public async Task GetDataCopy(string arg = "")
        {
            var userId = Context.User.Id;

            if (!_gudp.GlobalDataExists(userId)) return;

            var data = _gudp.GetGlobalUserData(userId);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            var template = _lang.Resolve($"{Context.User.Mention}\n[PRIVACY_DATA_REPORT_TEMPLATE(@DATA)]");

            var dataReport = string.Format(template, json);

            if (arg == "public")
            {
                await ReplyAsync(dataReport);
                return;
            }

            try
            {
                await Context.User.SendMessageAsync(_lang.Resolve($"{dataReport}\n[DM_DELETABLE]"));
                await ReplyAsync(_lang.Resolve($"{Context.User.Mention}, [PRIVACY_DATA_REPORT_SUCCESS]"));
            }
            catch (Exception)
            {
                await ReplyAsync(_lang.GetResource("PRIVACY_DATA_REPORT_FAIL"));
            }
        }

        [Command("data-delete")]
        [Summary("SUMMARY_DATA_DELETE")]
        [RequireDataCollectionConsent]
        public async Task DeleteDataAndConsent()
        {
            var userId = Context.User.Id;

            if (!_gudp.GlobalDataExists(userId)) return;

            _gudp.DeleteUserGlobalData(userId);

            await ReplyAsync(_lang.Resolve("[PRIVACY_MESSAGE_HEADER]\n\n[PRIVACY_DELETE_RESPONSE]"));
        }
    }
}