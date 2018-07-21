using JustineCore.Entities;

namespace JustineCore.Discord.Features.RPG
{
    public class RpgItem
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public Use UseDelegate { get; set; }
    }
}