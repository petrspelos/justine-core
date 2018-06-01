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
        private readonly RpgRepository _rpgItemRepository;

        public RpgStatus(GlobalUserDataProvider userProvider, RpgRepository rpgItemRepository)
        {
            _userProvider = userProvider;
            _rpgItemRepository = rpgItemRepository;
        }

        [Command("stats")]
        [RequireDataCollectionConsent]
        public async Task ShowStats()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);

            await ReplyAsync($@"{Context.User.Mention}
```
Gold: {gold}

[N/A] Health: ..... {globalUser.RpgAccount.Health} / {globalUser.RpgAccount.MaxHealth}
[STR] Strength: ... {globalUser.RpgAccount.Strength} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Strength + 1))} gold to upgrade.
[SPD] Speed: ...... {globalUser.RpgAccount.Speed} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Speed + 1))} gold to upgrade.
[INT] Intelligence: {globalUser.RpgAccount.Intelligence} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Intelligence + 1))} gold to upgrade.
[END] Endurance: .. {globalUser.RpgAccount.Endurance} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Endurance + 1))} gold to upgrade.
[LCK] Luck: ....... {globalUser.RpgAccount.Luck} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Luck + 1))} gold to upgrade.
```

`[Mention/Prefix] upgrade STR` to upgrade strength. (where STR is the stat's shortcut)

`[Mention/Prefix] heal` to get 50 HP for 50 gold.");
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
        [RequireRpgAlive]
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

            // Dig up a guy chance
            var deadPlayers = _userProvider.SearchByPredicate(u => u.RpgAccount.Health <= 0);
            if(deadPlayers.Count() > 0 &&
                r.Next(0, 101) > 10)
            {
                var luckyGuy = Utility.GetRandomElement(deadPlayers.ToList());
                var luckyUser = Context.Guild.GetUser(luckyGuy.DiscordId);
                var luckyMention = (luckyUser == null) ? "someone from a different server" : luckyUser.Mention;

                await ReplyAsync($":coffin: Oh damn! {Context.User.Mention} you dug up {luckyMention}.\n\nThanks to you, they get +1 health, which makes them alive again. :thumbsup:");

                luckyGuy.RpgAccount.Health = 1;
                _userProvider.SaveGlobalUserData(luckyGuy);
            }

            Logger.Log($"[Gold Digging] {Context.User.Username} - dug up {foundGold} gold.", ConsoleColor.Cyan);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            user.RpgAccount.AddItemById(goldId, foundGold);
            _userProvider.SaveGlobalUserData(user);
            
            var reportMsg = $":mega: {Context.User.Mention} found **{foundGold} gold**!";

            if (foundGold <= 5) reportMsg += "<a:YouTried:438951533971898369>";

            await ReplyAsync(reportMsg);
        }

        // =============================
        // Upgrades
        // =============================
        [Command("upgrade")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task UpgradeStat(string stat)
        {
            stat = stat.ToLower();
            if(!Constants.ValidUpgradeLabels.Contains(stat))
            {
                await ReplyAsync($"Sorry, '{stat}' does not appear to be a valid upgrade label.\n\nTry `[Mention/Prefix] stats` to see the list.");
                return;
            }

            var user = _userProvider.GetGlobalUserData(Context.User.Id);

            var result = StatUpgrade.StatUpgradeResult.NotEnoughGold;
            if(stat == "str")
            {
                result = user.RpgAccount.UpgradeStrength();
            }
            else if(stat == "hp")
            {
                result = user.RpgAccount.UpgradeHealth();
            }
            else if(stat == "spd")
            {
                result = user.RpgAccount.UpgradeSpeed();
            }
            else if(stat == "lck")
            {
                result = user.RpgAccount.UpgradeLuck();
            }
            else if(stat == "int")
            {
                result = user.RpgAccount.UpgradeIntelligence();
            }
            else// if(stat == "end")
            {
                result = user.RpgAccount.UpgradeEndurance();
            }

            if(result == StatUpgrade.StatUpgradeResult.NotEnoughGold)
            {
                await ReplyAsync($"You don't have enough gold to upgrade that stat.\n\nSee `[Mention/Prefix] stats` for more details.");
                return;
            }
            else
            {
                _userProvider.SaveGlobalUserData(user);
                await ReplyAsync($"{Context.User.Mention}, :white_check_mark: Success!");
            }
        }

        [Command("heal")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task Heal()
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);

            var result = user.RpgAccount.HealFor50();
            
            if(result == StatUpgrade.StatUpgradeResult.NotEnoughGold)
            {
                await ReplyAsync($"You don't have enough gold to heal.\n\nYou need 50 gold. _(Healing will add 50 HP)_");
                return;
            }
            else
            {
                _userProvider.SaveGlobalUserData(user);
                await ReplyAsync($"{Context.User.Mention}, :white_check_mark: Success, you were healed for 50 HP! (This does not overflow your max health)");
            }
        }

        [Command("resurrect")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task Resurrect(IGuildUser target)
        {
            if(!_userProvider.GlobalDataExists(target.Id))
            {
                await ReplyAsync($"Unfortunately, {target.Username} does not play this game. =/");
                return;
            }

            var user = _userProvider.GetGlobalUserData(Context.User.Id);
            var targetUser = _userProvider.GetGlobalUserData(target.Id);

            if(targetUser.RpgAccount.Health < 0)
            {
                await ReplyAsync($"That's very sweet of you, but {target.Username} is still alive... You cannot resurrect living people.");
                return;
            }

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = user.RpgAccount.GetItemCount(goldId);

            if(gold < 50)
            {
                await ReplyAsync($"You don't have enough gold to resurrect.\n\nYou need 50 gold.");
                return;
            }
            else
            {
                user.RpgAccount.RemoveItemCount(goldId, 50);
                targetUser.RpgAccount.Health = 1;
                _userProvider.SaveGlobalUserData(user);
                _userProvider.SaveGlobalUserData(targetUser);
                await ReplyAsync($"{Context.User.Mention}, :white_check_mark: you managed to resurrect {target.Mention} for 50 gold.");
            }
        }

        // =============================
        // Short Missions
        // =============================
        [Command("mission short")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task ShortMission()
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);

            if (user.RpgAccount.OnAdventure) return;

            user.RpgAccount.OnAdventure = true;

            var failChance = 15; // base failChance;
            
            if(user.RpgAccount.Health < 15)
            {
                failChance += 20;
            }
            else if(user.RpgAccount.Health < 40)
            {
                failChance += 5;
            }

            failChance -= Utility.Random.Next(0, (int)user.RpgAccount.Luck);

            var failRoll = Utility.Random.Next(1, 101);
            bool success = failRoll > failChance;

            var maxReward = 20;

            var luckRewardMod = Utility.Random.Next(0, (int)user.RpgAccount.Luck);
            var intelligenceRewardMod = Utility.Random.Next(0, Utility.GetLogValNoNegative((int)user.RpgAccount.Intelligence));

            
            maxReward += luckRewardMod;
            maxReward += intelligenceRewardMod;

            var reward = Utility.Random.Next(10, maxReward);

            var damageTaken = Utility.Random.Next(5, 20);
            var damageBase = damageTaken;

            if(!success) damageTaken = damageTaken * 2;

            var enduranceAbsorbtionPotential = Utility.GetLogValNoNegative((int)user.RpgAccount.Endurance);
            var enduranceAbsorbtion = Utility.Random.Next(0, enduranceAbsorbtionPotential);
            damageTaken = Math.Clamp(damageTaken - enduranceAbsorbtion, 1, int.MaxValue);
            
            Console.WriteLine($@"{Context.User.Username} rolled the following:
-- FAIL CHANCE --
Chance: {failChance}
Roll: {failRoll}
Failed: {(success ? "False" : "True")}

-- REWARD --
Luck: {luckRewardMod}
Intelligence: {intelligenceRewardMod}
-- REWARD TOTAL: {reward} --

-- DAMAGE --
Damage base: {damageBase}
Endurance absorbtion potential: {enduranceAbsorbtionPotential}
Endurance absorbtion: {enduranceAbsorbtion}
-- DAMAGE TOTAL: {damageTaken} --
");

            await ReplyAsync($"{Context.User.Mention}, I'll notify you when you're back from the adventure... (in about 5 minutes)");
            
            Utility.ExecuteAfter(async () => {
                user.RpgAccount.Health = Math.Clamp(user.RpgAccount.Health - damageTaken, 0, user.RpgAccount.MaxHealth);
                if(!success)
                {
                    await ReplyAsync($@"{Context.User.Mention},

:x: **Your mission failed.**

{Utility.GetRandomElement(Constants.MissionFailCauses.ToList())}

:heart: -{damageTaken}");
                }
                else
                {
                    user.RpgAccount.AddItemById(1, (uint)reward);
                    await ReplyAsync($@"{Context.User.Mention},

:white_check_mark: **Your mission was a success!**

{Utility.GetRandomElement(Constants.MissionSuccessCauses.ToList())}

:heart: -{damageTaken}
:moneybag: {reward}");
                }
                user.RpgAccount.OnAdventure = false;
                _userProvider.SaveGlobalUserData(user);
            }, 60 * 5);
        }
    }
}