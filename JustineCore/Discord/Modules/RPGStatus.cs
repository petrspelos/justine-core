using Discord;
using Discord.Commands;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Features.RPG.Actions;
using JustineCore.Discord.Features.RPG.GoldDigging;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JustineCore.Discord.Modules
{
    public class RpgStatus : ModuleBase<SocketCommandContext>
    {
        private readonly GlobalUserDataProvider _userProvider;
        private readonly RpgRepository _rpgItemRepository;
        private readonly DiggingJobProvider _djp;

        public RpgStatus(GlobalUserDataProvider userProvider, RpgRepository rpgItemRepository, DiggingJobProvider digJobProv)
        {
            _userProvider = userProvider;
            _rpgItemRepository = rpgItemRepository;
            _djp = digJobProv;
        }

        [Command("stats")]
        [RequireDataCollectionConsent]
        public async Task ShowStats()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            await ReplyAsync($@"{Context.User.Mention}
{GetRpgAccountStatsReport(globalUser)}

`[Mention/Prefix] upgrade STR` to upgrade strength. (where STR is the stat's shortcut)

`[Mention/Prefix] heal` to get 50 HP for 50 gold.");
        }

        [Command("stats")]
        [RequireDataCollectionConsent]
        public async Task ShowStats(IGuildUser target)
        {
            var nickOrUsername = target.Nickname??target.Username;

            if(target.Id == Context.User.Id)
            {
                await ShowStats();
                return;
            }

            if(target.IsBot)
            {
                await ReplyAsync("Bots don't have stats. Even if they had, they would beat you up.");
                return;
            }

            if(!_userProvider.GlobalDataExists(target.Id))
            {
                await ReplyAsync($"{nickOrUsername} did not give data collection consent.");
                return;
            }

            var globalUser = _userProvider.GetGlobalUserData(target.Id);

            await ReplyAsync($@"Stats of {nickOrUsername}
            {GetRpgAccountStatsReport(globalUser)}");
        }

        private string GetRpgAccountStatsReport(GlobalUserData globalUser)
        {
            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);

            return $@"```
Gold: {gold}

[N/A] Health: ..... {globalUser.RpgAccount.Health} / {globalUser.RpgAccount.MaxHealth}
[STR] Strength: ... {globalUser.RpgAccount.Strength} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Strength + 1))} gold to upgrade.
[SPD] Speed: ...... {globalUser.RpgAccount.Speed} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Speed + 1))} gold to upgrade.
[INT] Intelligence: {globalUser.RpgAccount.Intelligence} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Intelligence + 1))} gold to upgrade.
[END] Endurance: .. {globalUser.RpgAccount.Endurance} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Endurance + 1))} gold to upgrade.
[LCK] Luck: ....... {globalUser.RpgAccount.Luck} | {Utility.GetGeneralCurveCost((int)(globalUser.RpgAccount.Luck + 1))} gold to upgrade.
```";
        }

        [Command("gold")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task CheckRpgGold()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);

            await ReplyAsync($"{Context.User.Mention}, you have {gold} gold.");
        }

        [Command("gold")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
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

        [Command("debug")]
        [RequireDataCollectionConsent]
        [RequireOwner]
        public async Task Debug(int minutes)
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);

            

            await ReplyAsync("Success!");
        }
        
        // =============================
        // Single player digging
        // =============================

        [Command("gold dig cancel")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task CancelGoldDigging()
        {
            if (!_djp.IsDigging(Context.User.Id))
            {
                await ReplyAsync($"{Context.User.Mention}, you are not currently digging.");
                return;
            }

            _djp.RemoveByUserId(Context.User.Id);

            await ReplyAsync($"{Context.User.Mention}, you cancelled your digging and will not get any reward.");
        }

        [Command("gold dig reward")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task CollectDiggingReward()
        {
            if (!_djp.IsDigging(Context.User.Id))
            {
                await ReplyAsync($"{Context.User.Mention}, you need to start digging before you attempt to collect your reward.");
                return;
            }

            var job = _djp.Get(j => j.UserId == Context.User.Id).FirstOrDefault();

            if(!job.IsComplete())
            {
                await ReplyAsync($"{Context.User.Mention}, you are not done digging. You will be notified when your digging comes to an end.");
                return;
            }

            var reward = (uint)job.GetReward();

            var user = _userProvider.GetGlobalUserData(Context.User.Id);
            user.RpgAccount.AddGold(reward);
            _userProvider.SaveGlobalUserData(user);

            _djp.RemoveByUserId(Context.User.Id);

            await ReplyAsync($"{Context.User.Mention}, you collected {reward} gold for your digging.");
        }

        [Command("gold dig")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        public async Task SearchForGold(int hours = 1)
        {
            var user = _userProvider.GetGlobalUserData(Context.User.Id);
            if (user.RpgAccount.OnAdventure) return;
            if(hours < 1)
            {
                await ReplyAsync("The minimal time to dig is 1 hour.");
                return;
            }
            else if(hours > 8)
            {
                await ReplyAsync("You cannot dig for more than 8 hours at a time.");
                return;
            }

            _djp.AddJob(new DiggingJob
            {
                UserId = Context.User.Id,
                DiggingLengthInHours = hours,
                GuildId = Context.Guild.Id,
                TextChannelId = Context.Channel.Id,
                StartDateTime = DateTime.Now
            });

            var diggingPhrases = new []
            {
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on this cool mountain path._ :mountain_snow: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the depths of a dark city._ :night_with_stars: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in Sweden._ :flag_se: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the moon._ :waning_gibbous_moon: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the local fountain._ :fountain: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in a video game._ :space_invader: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in a chair factory._ :factory: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in..._ <a:ExcuseMeWtf:448550985447637002> :wedding: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on a roller coaster (good luck)._ :roller_coaster: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the beach._ :beach_umbrella: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the wilderness._ :tent: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _in the bathtub._ :bathtub: They'll be back in {hours} hour(s).",
                $"<:travel:449271721522888744> {Context.User.Mention} decided to search for some **gold** _on the Internet._ :computer: They'll be back in {hours} hour(s)."
            };

            await ReplyAsync(Utility.GetRandomElement(diggingPhrases.ToList()));

            Logger.Log($"[Gold Digging] {Context.User.Username} - for {hours} hours", ConsoleColor.Cyan);
        }

        // =============================
        // Upgrades
        // =============================
        [Command("upgrade")]
        [RequireDataCollectionConsent]
        [RequireRpgAlive]
        [RequireRpgNotGoldDigging]
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
        [RequireRpgNotGoldDigging]
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
        [RequireRpgNotGoldDigging]
        public async Task Resurrect(IGuildUser target)
        {
            if(!_userProvider.GlobalDataExists(target.Id))
            {
                await ReplyAsync($"Unfortunately, {target.Username} does not play this game. =/");
                return;
            }

            var user = _userProvider.GetGlobalUserData(Context.User.Id);
            var targetUser = _userProvider.GetGlobalUserData(target.Id);

            if(targetUser.RpgAccount.Health > 0)
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
        [RequireRpgNotGoldDigging]
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

            var reward = (uint)Utility.Random.Next(10, maxReward);

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

            await ReplyAsync($@"{Context.User.Mention},

You embark on an epic 5 minutes long mission.

Your task is {Utility.GetRandomElement(Constants.MissionPitches.ToList())}");
            
            Utility.ExecuteAfter(async () => {
                
                if(!_userProvider.GlobalDataExists(Context.User.Id)) return;
                
                var rpgUser = user.RpgAccount;

                rpgUser.GiveDamage(damageTaken);

                if(!success)
                {
                    await ReplyAsync($@"{Context.User.Mention},

:x: **Your mission failed.** 

{Utility.GetRandomElement(Constants.MissionFailCauses.ToList())} :coffin:

:heart: -{damageTaken}");
                }
                else
                {
                    rpgUser.AddGold(reward);

                    await ReplyAsync($@"{Context.User.Mention},

:white_check_mark: **Your mission was a success!**

{Utility.GetRandomElement(Constants.MissionSuccessCauses.ToList())}

:heart: -{damageTaken} | :moneybag: {reward}

You now have **{rpgUser.Health} HP** and **{rpgUser.GetGoldAmount()} gold**.");
                }

                rpgUser.OnAdventure = false;

                _userProvider.SaveGlobalUserData(user);
            }, 60 * 5);
        }
    }
}