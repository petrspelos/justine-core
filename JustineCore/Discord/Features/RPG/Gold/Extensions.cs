using System;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Features.RPG.Actions;

namespace JustineCore.Discord.Features.RPG.Gold
{
    public static class GoldExtensions
    {
        private static readonly uint GoldId;

        static GoldExtensions()
        {
            var repo = Unity.Resolve<RpgRepository>();
            GoldId = repo.GetItemByName("gold").Id;
        }

        public static uint GetGoldAmount(this RpgAccount account)
        {
            return account.GetItemCount(GoldId);
        }

        public static bool HasEnoughGold(this RpgAccount account, uint minimum)
        {
            return account.GetGoldAmount() >= minimum;
        }

        public static void AddGold(this RpgAccount account, uint amount)
        {
            account.AddItemById(GoldId, amount);
        }

        /// <summary>Remove gold from an RpgAccount.</summary>
        /// <exception cref="NotEnoughGoldException"></exception>
        public static void RemoveGold(this RpgAccount account, uint amount)
        {
            if(!account.HasEnoughGold(amount)) throw new NotEnoughGoldException();

            account.RemoveItemCount(GoldId, amount);
        }

        /// <summary>Transfers gold from one RpgAccount to another.</summary>
        /// <exception cref="NotEnoughGoldException"></exception>
        public static void TransferGold(this RpgAccount source, RpgAccount target, uint amount)
        {
            if(!source.HasEnoughGold(amount)) throw new NotEnoughGoldException();

            source.RemoveGold(amount);
            target.AddGold(amount);
        }
    }
}
