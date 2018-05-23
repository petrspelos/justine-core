using System.Linq;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class InventoryUtils
    {
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
        public static int GetItemCount(this RpgAccount accout, uint itemId)
        {
            return (int) accout.InventorySlots.Where(s => s.Item.Id == itemId).Sum(s => s.Amount);
        }
    }
}