using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using JustineCore.Discord.Preconditions;
using System.Threading.Tasks;
using JustineCore.Configuration;
using JustineCore.Discord.Features.Payloads;
using Newtonsoft.Json;

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

        [Command("date")]
        public async Task GetDate()
        {
            await ReplyAsync($"Today is {Utility.ResolvePlaceholders("<proper-date>")}.");
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

        [Command("schtasks")]
        [RequireOwner]
        public async Task GetSchtasks()
        {
            var schMessages = AppConfig.DiscordBotConfig.ScheduledMessages;
            var msg = new StringBuilder();
            foreach (var sm in schMessages)
            {
                msg.Append($"**Every day at ** {sm.Hour}:{sm.Minute}\n");
                foreach (var mPayload in sm.Payloads)
                {
                    var targetGuild = Context.Client.GetGuild(mPayload.GuildId);
                    msg.Append($"\t**To:** {targetGuild.Name} : {targetGuild.GetTextChannel(mPayload.ChannelId).Name}\n");
                    msg.Append($"\t**Message:**\n```\n{mPayload.MessageTemplate}\n```\n");
                }
            }

            await ReplyAsync(msg.ToString());
        }

        [Command("schtasks config")]
        [RequireOwner]
        public async Task GetConfig()
        {
            if (AppConfig.DiscordBotConfig.ScheduledMessages.Count == 0)
                await ReplyAsync("There are no scheduled messages.");

            var json = JsonConvert.SerializeObject(AppConfig.DiscordBotConfig.ScheduledMessages, Formatting.Indented);

            await ReplyAsync($"```json\n{json}\n```");
        }

        [Command("schtasks config")]
        [RequireOwner]
        public async Task GetConfig([Remainder]string newJson)
        {
            var newSchtasks = new List<ScheduledMessage>();
            try
            {
                newSchtasks = JsonConvert.DeserializeObject<List<ScheduledMessage>>(newJson);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"there's a problem with your JSON. {e.Message}");
            }

            foreach (var sm in newSchtasks)
            {
                if (sm.Hour < 0 || sm.Hour > 23) throw new ArgumentOutOfRangeException("the hour must be between 0 and 23.");
                if (sm.Minute < 0 || sm.Minute > 59) throw new ArgumentOutOfRangeException("the minute must be between 0 and 59");

                foreach (var p in sm.Payloads)
                {
                    var g = Context.Client.GetGuild(p.GuildId);
                    if (g is null) throw new ArgumentException("the guild ID you mentioned is unknown to me");
                    if (g.GetTextChannel(p.ChannelId) is null) throw new ArgumentException("the text channel ID you mentioned is unknown to me");
                }
            }

            AppConfig.DiscordBotConfig.ScheduledMessages = newSchtasks;
            AppConfig.StoreCurrentBotConfig();
            
            Program.Connection.ReloadAllScheduledTasks();

            await ReplyAsync("New config accepted.");
        }
    }
}
