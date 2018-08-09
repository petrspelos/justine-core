using System.Linq;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Features.RPG.Actions;
using Xunit;

namespace JustineCore.Tests
{
    public class RpgTransactionTests
    {
        [Fact]
        public void ForceAddItem_ValidTest()
        {
            var acc = new RpgAccount();

            Assert.True(acc.InventorySlots.Count == 0);

            acc.ForceAddItemById(itemId: 1, amount: 5);

            Assert.True(acc.InventorySlots.Count == 1);
            Assert.True(acc.InventorySlots.First().Amount == 5);
            Assert.True(acc.InventorySlots.First().Item.Id == 1);
        }

        [Fact]
        public void ForceAddItem_ItemMergeTest()
        {
            var acc = new RpgAccount();
            acc.ForceAddItemById(itemId: 2, amount: 5);

            Assert.True(acc.InventorySlots.Count == 1);

            acc.ForceAddItemById(itemId: 2, amount: 5);

            Assert.True(acc.InventorySlots.Count == 1);
            Assert.True(acc.InventorySlots.First().Amount == 10);
            Assert.True(acc.InventorySlots.First().Item.Id == 2);
        }
    }
}