using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
using JustineCore.Language;
using Newtonsoft.Json;

namespace JustineCore.Discord.Modules
{
    public class Privacy : ModuleBase<SocketCommandContext>
    {
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
            
            await ReplyAsync($"{Context.User.Mention}\n{Localization.GetResource("PRIVACY_AGREE_RESPONSE")}");
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
            
            var dataReport = string.Format($"{Context.User.Mention}\n{Localization.GetResource("PRIVACY_DATA_REPORT_TEMPLATE(@DATA)")}", json);

            if (arg == "public")
            {
                await ReplyAsync(dataReport);
                return;
            }

            try
            {
                await Context.User.SendMessageAsync(dataReport);
                await ReplyAsync($"{Context.User.Mention}, {Localization.GetResource("PRIVACY_DATA_REPORT_SUCCESS")}");
            }
            catch (Exception)
            {
                await ReplyAsync(Localization.GetResource("PRIVACY_DATA_REPORT_FAIL"));
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

            await ReplyAsync($"{Context.User.Mention}\n{Localization.GetResource("PRIVACY_DELETE_RESPONSE")}");
        }
    }
}