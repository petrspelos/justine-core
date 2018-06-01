using System;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;

namespace JustineCore.Discord.Preconditions
{
    public class RequireRpgAlive : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = context.User.Id;

            if (!gudp.GlobalDataExists(userId))
            {
                return Task.FromResult(PreconditionResult.FromError("You must consent to data collection in order to use this feature."));
            }

            var user = gudp.GetGlobalUserData(userId);

            if (user.RpgAccount.Health <= 0)
            {
                return Task.FromResult(PreconditionResult.FromError($"you have to be alive to do that.\n\n:coffin: Someone else can use `[Mention/Prefix] resurrect {context.User.Mention}` to resurrect you for 20 gold.\n\nBut your friends most likely buried already."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}