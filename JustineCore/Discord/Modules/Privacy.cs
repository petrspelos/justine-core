using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
using Newtonsoft.Json;

namespace JustineCore.Discord.Modules
{
    public class Privacy : ModuleBase<SocketCommandContext>
    {
        [Command("I agree with my data being collected by Justine.")]
        [Summary("Giving consent to data collection for the purposes of providing features.")]
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

            const string consentMessage = @":white_check_mark: **Your data collection settings were updated**
I can now collect data about you in order to provide some features, such as profiles or economy.
However, you are still in control of your data. Here are some commands you can use to work with your data:
`data-view` or `data-show` to get a Direct Message with all data collected about you.
`data-delete` to delete all collected data. :warning: **This action cannot be taken back!**";

            await ReplyAsync($"{Context.User.Mention}\n{consentMessage}");
        }

        [Command("data-view")]
        [Alias("data-show")]
        [Summary("Returns a direct unedited copy of all data collected about you in json form.")]
        [RequireDataCollectionConsent]
        public async Task GetDataCopy(string arg = "")
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = Context.User.Id;

            if (!gudp.GlobalDataExists(userId)) return;

            var data = gudp.GetGlobalUserData(userId);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            const string dataReportFormat = @":clipboard: **Data collection report**
_Please check the time this message was sent to you. The older the message, the higher the chance of the information being outdated._
Here is the data collection report you requested:
```json
{0}
```";
            var dataReport = string.Format($"{Context.User.Mention}\n{dataReportFormat}", json);

            if (arg == "public")
            {
                await ReplyAsync(dataReport);
                return;
            }

            try
            {
                await Context.User.SendMessageAsync(dataReport);
                await ReplyAsync($"{Context.User.Mention}, a DM with the report was sent to you.");
            }
            catch (Exception)
            {
                await ReplyAsync(@":thinking: **The message couldn't be sent**
I wasn't able to DM you your data collection report. This usually happens when a user has their DMs disabled. Please make sure people not in your friends list can message you.
If you want, I can send your information here, **however**, keep in mind everyone in this channel will be able to see your information!
On the other hand, you will be able to delete it if you have enough privileges (unlike DMs).
To force public report, use `data-view public`. **But really think about it before you do it!**");
            }
        }

        [Command("data-delete")]
        [Summary("Deletes all collected data about you and also removes your consent to further data collection. (You cannot take this action back)")]
        [RequireDataCollectionConsent]
        public async Task DeleteDataAndConsent()
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = Context.User.Id;

            if (!gudp.GlobalDataExists(userId)) return;

            gudp.DeleteUserGlobalData(userId);

            const string consentMessage = @":white_check_mark: **Your data collection settings were updated**
I deleted all data I've collected about you. You now cannot take this action back.
Your consent was also removed and I will not continue collecting your data unless you specifically give consent again.
Should you find the need for some data collection dependent features again, feel free to give your consent.

:slight_smile: _You were my favorite, ... ehm... I forgot your name..._";

            await ReplyAsync($"{Context.User.Mention}\n{consentMessage}");
        }
    }
}