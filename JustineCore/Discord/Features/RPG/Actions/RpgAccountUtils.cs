using System;
using JustineCore.Discord.Features.RPG.Gold;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class RpgAccountUtils
    {
        public static void GiveDamage(this RpgAccount account, int damage)
        {
            account.Health = Math.Clamp(account.Health - damage, 0, account.MaxHealth);
        }

        public static string GetStringReport(this RpgAccount account)
        {
            var gold = account.GetGoldAmount();

            return $@"```
Gold: {gold}

[N/A] Health: ..... {account.Health} / {account.MaxHealth}
[STR] Strength: ... {account.Strength} | {Utility.GetGeneralCurveCost(account.Strength + 1)} gold to upgrade.
[SPD] Speed: ...... {account.Speed} | {Utility.GetGeneralCurveCost(account.Speed + 1)} gold to upgrade.
[INT] Intelligence: {account.Intelligence} | {Utility.GetGeneralCurveCost(account.Intelligence + 1)} gold to upgrade.
[END] Endurance: .. {account.Endurance} | {Utility.GetGeneralCurveCost(account.Endurance + 1)} gold to upgrade.
[LCK] Luck: ....... {account.Luck} | {Utility.GetGeneralCurveCost(account.Luck + 1)} gold to upgrade.
```";
        }
    }
}
