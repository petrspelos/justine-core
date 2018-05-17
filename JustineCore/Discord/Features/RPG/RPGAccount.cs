using System.Collections.Generic;

namespace JustineCore.Discord.Features.RPG
{
    public class RpgAccount
    {
        public uint Strength { get; set; }
        public uint Dexterity { get; set; }
        public List<InventorySlot> InventorySlots { get; set; } = new List<InventorySlot>();
    }
}