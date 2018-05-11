using System;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;

namespace JustineCore.Discord.Preconditions
{
    public class RequireDataCollectionConsent : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var localization = Unity.Resolve<ILocalization>();
            var userId = context.User.Id;

            if (!gudp.GlobalDataExists(userId))
            {
                return Task.FromResult(PreconditionResult.FromError(localization.Resolve("[PRECONDITION_COLLECTION_DENIED]")));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}