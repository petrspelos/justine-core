using System;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Features.RPG.GoldDigging;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;

namespace JustineCore.Discord.Preconditions
{
    public class RequireRpgNotGoldDigging : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var djp = Unity.Resolve<DiggingJobProvider>();

            var userId = context.User.Id;

            if (!gudp.GlobalDataExists(userId))
            {
                return Task.FromResult(PreconditionResult.FromError("You must consent to data collection in order to use this feature."));
            }

            var user = gudp.GetGlobalUserData(userId);

            if (djp.IsDigging(userId))
            {
                return Task.FromResult(PreconditionResult.FromError($"you cannot perform this action while digging for gold.\n\nYou can, however, cancel your digging with `[Mention/Prefix] gold dig cancel`"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}