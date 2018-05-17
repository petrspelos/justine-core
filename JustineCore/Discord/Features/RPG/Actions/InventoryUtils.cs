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
    }
}