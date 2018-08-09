using System;
using System.Linq;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class StatUpgrade
    {
        public enum StatUpgradeResult { Success, NotEnoughGold }

        public static StatUpgradeResult UpgradeStrength(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.Strength + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Strength++;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult UpgradeHealth(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.MaxHealth + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.MaxHealth += 25;
            account.Health += 25;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult UpgradeSpeed(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.Speed + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Speed++;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult UpgradeLuck(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.Luck + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Luck++;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult UpgradeIntelligence(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.Intelligence + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Intelligence++;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult UpgradeEndurance(this RpgAccount account)
        {
            var cost = JustineCore.Utilities.GetGeneralCurveCost((int)(account.Endurance + 1));
            var gold = account.GetItemCount(1);

            if(cost > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Endurance++;

            return StatUpgradeResult.Success;
        }

        public static StatUpgradeResult HealFor50(this RpgAccount account)
        {
            var cost = 50;
            var gold = account.GetItemCount(1);

            if(50 > gold) return StatUpgradeResult.NotEnoughGold;

            account.RemoveItemCount(1, (uint)cost);
            account.Health = Math.Clamp(account.Health + 50, 0, account.MaxHealth);

            return StatUpgradeResult.Success;
        }

        public static void AddHealth(this RpgAccount account, int amount)
        {
            account.Health = Math.Clamp(account.Health + amount, 0, account.MaxHealth);
        }
    }
}