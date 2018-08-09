using System.Collections.Generic;
using Newtonsoft.Json;

namespace JustineCore.Discord.Features.RPG
{
    public class RpgAccount
    {
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public uint Strength { get; set; }
        public uint Speed { get; set; }
        public uint Luck { get; set; }
        public uint Intelligence { get; set; }
        public uint Endurance { get; set; }
        public int SlotLimit { get; set; } = 5;
        public List<InventorySlot> InventorySlots { get; set; } = new List<InventorySlot>();

        [JsonIgnore]
        public bool OnAdventure { get; set; }
    }
}   