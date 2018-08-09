using System;
using System.Threading.Tasks;
using Discord.Commands;
using JustineCore.Discord.Providers.UserData;
using JustineCore.Language;

namespace JustineCore.Discord.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireChannelById : PreconditionAttribute
    {
        private readonly ulong _requiredId;

        public RequireChannelById(ulong requiredId)
        {
            _requiredId = requiredId;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel.Id == _requiredId)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("You cannot use this command in this Channel."));
        }
    }
}