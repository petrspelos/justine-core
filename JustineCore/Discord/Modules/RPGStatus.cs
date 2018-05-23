using Discord.Commands;
using JustineCore.Discord.Features.RPG.Actions;
using JustineCore.Discord.Preconditions;
using JustineCore.Discord.Providers.UserData;
using System.Threading.Tasks;
using JustineCore.Discord.Features.RPG;

namespace JustineCore.Discord.Modules
{
    public class RpgStatus : ModuleBase<SocketCommandContext>
    {
        private readonly GlobalUserDataProvider _userProvider;
        private readonly RpgItemRepository _rpgItemRepository;

        public RpgStatus(GlobalUserDataProvider userProvider, RpgItemRepository rpgItemRepository)
        {
            _userProvider = userProvider;
            _rpgItemRepository = rpgItemRepository;
        }

        [Command("gold")]
        [RequireDataCollectionConsent]
        public async Task CheckRpgGold()
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;
            var gold = globalUser.RpgAccount.GetItemCount(goldId);
            
            await ReplyAsync($"{Context.User.Mention}, you have {gold} gold.");
        }

        [Command("gold spawn")]
        [RequireDataCollectionConsent]
        [RequireOwner]
        public async Task OwnerGetGold(uint amount)
        {
            var globalUser = _userProvider.GetGlobalUserData(Context.User.Id);

            var goldId = _rpgItemRepository.GetItemByName("gold").Id;

            globalUser.RpgAccount.ForceAddItemById(goldId, amount);

            _userProvider.SaveGlobalUserData(globalUser);

            await ReplyAsync("Success");
        }
    }
}