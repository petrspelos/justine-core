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
        private readonly ILocalization _localization;

        public Privacy(ILocalization localization)
        {
            _localization = localization;
        }

        [Command("I agree with my data being collected by Justine.")]
        [Summary("SUMMARY_DATA_ACCEPT")]
        public async Task GiveConsent()
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = Context.User.Id;

            if (gudp.GlobalDataExists(userId)) return;

            var consent = new DataConsent
            {
                Date = DateTime.UtcNow,
                MessageId = Context.Message.Id
            };

            gudp.AddNewGlobalData(userId, consent);
            
            await ReplyAsync(_localization.FromTemplate($"{Context.User.Mention}\n[PRIVACY_AGREE_RESPONSE]"));
        }

        [Command("data-view")]
        [Alias("data-show")]
        [Summary("SUMMARY_DATA_VIEW")]
        [RequireDataCollectionConsent]
        public async Task GetDataCopy(string arg = "")
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = Context.User.Id;

            if (!gudp.GlobalDataExists(userId)) return;

            var data = gudp.GetGlobalUserData(userId);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            var template = _localization.FromTemplate($"{Context.User.Mention}\n[PRIVACY_DATA_REPORT_TEMPLATE(@DATA)]");

            var dataReport = string.Format(template, json);

            if (arg == "public")
            {
                await ReplyAsync(dataReport);
                return;
            }

            try
            {
                await Context.User.SendMessageAsync(dataReport);
                await ReplyAsync(_localization.FromTemplate($"{Context.User.Mention}, [PRIVACY_DATA_REPORT_SUCCESS]"));
            }
            catch (Exception)
            {
                await ReplyAsync(_localization.GetResource("PRIVACY_DATA_REPORT_FAIL"));
            }
        }

        [Command("data-delete")]
        [Summary("SUMMARY_DATA_DELETE")]
        [RequireDataCollectionConsent]
        public async Task DeleteDataAndConsent()
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = Context.User.Id;

            if (!gudp.GlobalDataExists(userId)) return;

            gudp.DeleteUserGlobalData(userId);

            await ReplyAsync(_localization.FromTemplate($"{Context.User.Mention}\n[PRIVACY_DELETE_RESPONSE]"));
        }
    }
}