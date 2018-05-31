using Discord;
using Discord.Commands;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Features.RPG.Actions;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JustineCore.Discord.Modules
{
    public class RpgStatus : ModuleBase<SocketCommandContext>
    {
        private readonly GlobalUserDataProvider _userProvider;
        private readonly RpgItemRepository _rpgItemRepository;

        public RpgStatus(GlobalUserDataProvider userProvider, RpgItemRepository rpgItemRepository)
        {
            _userProvider = userProvider;
            _rpgItemRepository = rpgItemRepository;
        }

        [Command("stats")]
        [RequireDataCollectionConsent]
        public async Task ShowStats()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            await ReplyAsync($"{Context.User.Mention}\n```\nStrength: {globalUser.RpgAccount.Strength}\nSpeed: {globalUser.RpgAccount.Speed}\nLuck: {globalUser.RpgAccount.Luck}\n```");
        }

        [Command("gold")]
        [RequireDataCollectionConsent]
        public async Task CheckRpgGold()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);

            await ReplyAsync($"{Context.User.Mention}, you have {gold} gold.");
        }

        [Command("gold")]
        [RequireDataCollectionConsent]
        public async Task CheckRpgGold(IGuildUser target)
        {
            var nickOrUsername = target.Nickname??target.Username;

            if(target.Id == Context.User.Id)
            {
                await CheckRpgGold();
                return;
            }

            if(target.IsBot)
            {
                await ReplyAsync("Bots don't have souls. And as it turns out, nor do they have gold.");
                return;
            }

            if(!_userProvider.GlobalDataExists(target.Id))
            {
                await ReplyAsync($"{nickOrUsername} did not give data collection consent.");
                return;
            }

            var globalUser = _userProvider.GetGlobalUserData(target.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);

            await ReplyAsync($"{nickOrUsername} has {gold} gold.");
        }

        [Command("gold spawn")]
        [RequireDataCollectionConsent]
        [RequireOwner]
        public async Task OwnerGetGold(uint amount)
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;

            globalUser.RpgAccount.ForceAddItemById(goldId, amount);

            _userProvider.SaveGlobalUserData(globalUser);

            await ReplyAsync("Success");
        }
        
        // =============================
        // Single player digging
        // =============================

        [Command("gold dig")]
        [RequireDataCollectionConsent]
        public async Task SearchForGold()
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);

            if (user.RpgAccount.OnAdventure) return;

            user.RpgAccount.OnAdventure = true;

            const int defaultLength = 30 * 60;

            var diggingLength = Math.Clamp(defaultLength - user.RpgAccount.Speed * 2, 0, defaultLength);

            var diggingPhrases = new []
            {
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on this cool mountain path._ :mountain_snow: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the depths of a dark city._ :night_with_stars: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in Sweden._ :flag_se: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the moon._ :waning_gibbous_moon: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the local fountain._ :fountain: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in a video game._ :space_invader: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in a chair factory._ :factory: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in..._ <a:ExcuseMeWtf:448550985447637002> :wedding: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on a roller coaster (good luck)._ :roller_coaster: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the beach._ :beach_umbrella: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the wilderness._ :tent: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the bathtub._ :bathtub: They'll be back in {diggingLength / 60} minutes.",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the Internet._ :computer: They'll be back in {diggingLength / 60} minutes."
            };

            await ReplyAsync(Utility.GetRandomElement(diggingPhrases.ToList()));

            Logger.Log($"[Gold Digging] {Context.User.Username} - for {diggingLength} seconds", ConsoleColor.Cyan);

            Utility.ExecuteAfter(FinishGoldSearch, (int)diggingLength);
        }

        private async void FinishGoldSearch()
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);
            user.RpgAccount.OnAdventure = false;

            var r = new Random();
            var foundGold = (uint)r.Next(0, 20 + r.Next(0, (int)(5 * user.RpgAccount.Luck)));

            Logger.Log($"[Gold Digging] {Context.User.Username} - dug up {foundGold} gold.", ConsoleColor.Cyan);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            user.RpgAccount.AddItemById(goldId, foundGold);
            _userProvider.SaveGlobalUserData(user);
            
            var reportMsg = $":mega: {Context.User.Mention} found **{foundGold} gold**!";

            if (foundGold <= 5) reportMsg += "<a:YouTried:438951533971898369>";

            await ReplyAsync(reportMsg);
        }
    }
}