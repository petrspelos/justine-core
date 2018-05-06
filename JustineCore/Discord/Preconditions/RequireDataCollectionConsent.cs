using System;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Providers.UserData;

namespace JustineCore.Discord.Preconditions
{
    public class RequireDataCollectionConsent : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var gudp = Unity.Resolve<GlobalUserDataProvider>();
            var userId = context.User.Id;

            if (!gudp.GlobalDataExists(userId))
            {
                return Task.FromResult(PreconditionResult.FromError(
                    "To use this command, I need your consent to data collection. Don't worry, you can get a copy of the collected data, or immediately delete it at any time.\nTo give consent, you need to mention me and say: `I agree with my data being collected by Justine.`. Make sure you mention me first and put a single space after the mention."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}