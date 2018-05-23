using System.Linq;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class Transaction
    {
        /// <summary>
        /// Force adds an item. This will bypass any slot capacity limitations, and not enough slots limitations.
        /// </summary>
        /// <param name="account">Target RPG account</param>
        /// <param name="itemId">ID of the desired item</param>
        /// <param name="amount">Amount of the item to be added</param>
        public static void ForceAddItemById(this RpgAccount account, uint itemId, uint amount)
        {
            if (account.HasItem(itemId))
            {
                var targetSlot = account.InventorySlots.FirstOrDefault(s => s.Amount != 0 && s.Item.Id == itemId);
                if (targetSlot is null) return;
                targetSlot.Amount += amount;
            }
            else
            {
                account.InventorySlots.Add(new InventorySlot
                {
                    Amount = amount,
                    Item = new InventoryItem(itemId)
                });
            }
        }

        /// <summary>
        /// Adds an item based on item ID.
        /// </summary>
        public static void AddItemById(this RpgAccount account, uint itemId, uint amount)
        {
            var targetSlot = account.InventorySlots.FirstOrDefault(s => s.Item.Id == itemId);
            if (targetSlot is null)
            {
                account.ForceAddItemById(itemId, amount);
                return;
            }

            targetSlot.Amount += amount;
        }
    }
}