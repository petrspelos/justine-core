using System.Linq;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class InventoryUtils
    {
        private static uint goldId;

        static InventoryUtils()
        {
            var repo = Unity.Resolve<RpgRepository>();
            goldId = repo.GetItemByName("gold").Id;
        }

        /// <summary>
        /// Determines if the account has an item with a particular ID.
        /// </summary>
        /// <param name="account">target account</param>
        /// <param name="itemId">item ID</param>
        /// <returns>true if the the account has an item with that ID.</returns>
        public static bool HasItem(this RpgAccount account, uint itemId)
        {
            return account.InventorySlots.Any(s => s.Amount != 0 && s.Item.Id == itemId);
        }

        /// <summary>
        /// Gets a number of items of a certain type owned. (0 if not found)
        /// </summary>
        public static uint GetItemCount(this RpgAccount accout, uint itemId)
        {
            return (uint)accout.InventorySlots.Where(s => s.Item.Id == itemId).Sum(s => s.Amount);
        }

        // public static int GetGoldAmount(this RpgAccount account)
        // {
        //     return (int)account.GetItemCount(goldId);
        // }

        // public static void AddGold(this RpgAccount account, uint amount)
        // {
        //     account.AddItemById(goldId, amount);
        // }
    }
}
