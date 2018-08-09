using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Language;

namespace JustineCore.Discord.Features.TutorialServer
{
    public class WaitingRoomService
    {
#if DEBUG
        private const ulong TutorialServerId = Constants.ServiceServerId;
        private const ulong WaitingRoomChannelId = Constants.ServiceChannelId;
        private const ulong MemberRoleId = Constants.ServiceRole1;
        private const ulong BotRoleId = Constants.ServiceRole1;
        private const ulong BotDevRoleId = Constants.ServiceRole1;
        private const ulong TutorialGeneralId = Constants.ServiceGeneralChannelId;
#else
        private const ulong TutorialServerId = Constants.TutorialServerId;
        private const ulong WaitingRoomChannelId = Constants.WaitingRoomChannelId;
        private const ulong MemberRoleId = Constants.TutorialMemberRoleId;
        private const ulong BotRoleId = Constants.TutorialBotRoleId;
        private const ulong TutorialGeneralId = Constants.TutorialGeneralId;
        private const ulong BotDevRoleId = Constants.TutorialBotDevRoleId;
#endif
        private List<ulong> AlertedUserIds;
        private SocketGuild _tutorialServer;
        private SocketTextChannel _waitingRoomChannel;
        private SocketTextChannel _generalChannel;
        private SocketRole _memberRole;
        private SocketRole _botRole;
        private SocketRole _botDevRole;
        private readonly DiscordSocketClient _client;
        private readonly ILocalization _lang;
        private readonly VerificationProvider _botVer;

        public WaitingRoomService(DiscordSocketClient client, ILocalization lang, VerificationProvider botVer)
        {
            _client = client;
            
            _lang = lang;

            _botVer = botVer;

            AlertedUserIds = new List<ulong>();
        }

        internal async Task MessageReceived(SocketMessage socketMessage)
        {
            if(_tutorialServer is null) InitializeResources();

            if (!(socketMessage is SocketUserMessage msg)) return;
            if (msg.Channel is SocketDMChannel) return;
            var context = new SocketCommandContext(_client, msg);
            if(context.Guild.Id != TutorialServerId) return;

            if(context.Channel.Id != WaitingRoomChannelId) return;
            if(context.User.IsBot) return;

            var isValidAgreement = MessageIsValidAgreement(context);
            var userIsMember = UserIsMember((SocketGuildUser)context.User);
            var userAlreadyAlerted = UserAlreadyAlerted(context.User.Id);

            if(!userAlreadyAlerted && !userIsMember && !isValidAgreement)
            {
                AlertedUserIds.Add(context.User.Id);
                var welcomeMsg = _lang.GetPooledResource("WAITING_ROOM_REMINDER");
                await _waitingRoomChannel.SendMessageAsync(welcomeMsg);
            }
        }

        internal async Task UserLeft(SocketGuildUser user)
        {
            if(user.IsBot && _botVer.IsVerified(user.Id))
            {
                var ownerId = _botVer.SearchByPredicate(d => d.BotId == user.Id && d.Verified == true).FirstOrDefault().OwnerId;
                var owner = _tutorialServer.GetUser(ownerId);

                if(owner is null)
                {
                    Logger.Log($"[WaitingRoomService] BOT ({user.Username} left the server but I couldn't find the owner with ID {ownerId}. No action will be done...)");
                    return;
                }

                await owner.RemoveRoleAsync(_botDevRole);

                await _generalChannel.SendMessageAsync($"<@!182941761801420802>, I noticed ({user.Username}) left, which is a BOT by {owner.Mention}. So I removed their C# BOT DEV role. :shield:");
            }
            else if(!user.IsBot)
            {
                var ownedBots = _botVer.SearchByPredicate(d => d.OwnerId == user.Id && d.Verified == true).ToList();

                if(!ownedBots.Any()) return;

                var kickedBots = new StringBuilder();
                foreach(var botVerification in ownedBots)
                {
                    var id = botVerification.BotId;
                    var bot = _tutorialServer.GetUser(id);
                    if(bot is null) continue;
                    kickedBots.Append($"{bot.Username} ");
                    await bot.KickAsync("The owner left.");
                }

                await _generalChannel.SendMessageAsync($"<@!182941761801420802>, I noticed {user.Username} left. This user's BOTs were also kicked. :shield:");
            }
        }

        internal async Task UserJoined(SocketGuildUser user)
        {
            if(_tutorialServer is null) InitializeResources();
            if(user.IsBot)
            {
                await IntroduceBotToServer(user);
                return;
            }
            if(user.Guild.Id != TutorialServerId) return;

            var embed = new EmbedBuilder()
                .AddField("WHILE YOU WAIT", _lang.GetResource("TUTORIAL_WELCOME_WAITING"))
                .AddField("A FUN SERVER FACT", _lang.GetPooledResource("TUTORIAL_FUN_SERVER_FACTS"))
                .WithColor(new Color(102, 187, 106))
                .Build();

            await _waitingRoomChannel.SendMessageAsync($"**WELCOME** {user.Mention}!", embed: embed);
        }

        private async Task IntroduceBotToServer(SocketGuildUser botUser)
        {
            await botUser.AddRoleAsync(_botRole);

            var embed = new EmbedBuilder()
                .AddField("VERIFICATION NEEDED", _lang.GetResource("TUTORIAL_WELCOME_BOT_REGISTER"))
                .AddField("C# PROOF NEEDED", _lang.GetResource("TUTORIAL_WELCOME_BOT_PROVECS"))
                .AddField("FAILURE TO COMPLETE", _lang.GetResource("TUTORIAL_WELCOME_BOT_FAILURE"))
                .WithColor(new Color(38, 166, 154))
                .Build();

            await _generalChannel.SendMessageAsync($"{botUser.Mention} <:bot:400105688967413781> was just added!", embed: embed);
        }

        private void InitializeResources()
        {
            _tutorialServer = _client.GetGuild(TutorialServerId);

            _waitingRoomChannel = _tutorialServer.GetTextChannel(WaitingRoomChannelId);

            _generalChannel = _tutorialServer.GetTextChannel(TutorialGeneralId);

            _memberRole = _tutorialServer.GetRole(MemberRoleId);

            _botRole = _tutorialServer.GetRole(BotRoleId);

            _botDevRole = _tutorialServer.GetRole(BotDevRoleId);
        }

        private bool MessageIsValidAgreement(SocketCommandContext context)
        {
            var containsPhrase = context.Message.Content.ToLower().Contains("i accept the rules");
            var justine = _tutorialServer.GetUser(Constants.JustineId);
            var mentionsJustine = context.Message.MentionedUsers.Contains(justine);
            return containsPhrase && mentionsJustine;
        }

        private bool UserIsMember(SocketGuildUser user)
        {
            return user.Roles.Contains(_memberRole);
        }

        private bool UserAlreadyAlerted(ulong userId)
        {
            return AlertedUserIds.Contains(userId);
        }
    }
}