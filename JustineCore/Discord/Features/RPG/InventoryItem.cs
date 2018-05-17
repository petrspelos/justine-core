namespace JustineCore.Discord.Features.RPG
{
    public struct InventoryItem
    {
        public uint Id { get; set; }

        public InventoryItem(uint itemId)
        {
            Id = itemId;
        }
    }
}