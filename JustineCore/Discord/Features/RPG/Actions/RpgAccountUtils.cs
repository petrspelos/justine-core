using System;

namespace JustineCore.Discord.Features.RPG.Actions
{
    public static class RpgAccountUtils
    {
        public static void GiveDamage(this RpgAccount account, int damage)
        {
            account.Health = Math.Clamp(account.Health - damage, 0, account.MaxHealth);
        }
    }
}
