using System.Collections.Generic;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;
using SessionTracker.Models.Constants;

namespace SessionTracker.Services.Api
{
    public class ApiService
    {
        public static bool ModuleHasApiToken(Gw2ApiManager gw2ApiManager)
        {
            return gw2ApiManager.HasPermissions(API_TOKEN_PERMISSIONS_EVERY_API_KEY_HAS_BY_DEFAULT);
        }

        public static bool ApiKeyIsMissingPermissions(Gw2ApiManager gw2ApiManager)
        {
            return !gw2ApiManager.HasPermissions(API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE);
        }

        public static async Task UpdateTotalValuesInModel(Model model, Gw2ApiManager gw2ApiManager)
        {
            var charactersTask      = gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var pvpStatsTask        = gw2ApiManager.Gw2ApiClient.V2.Pvp.Stats.GetAsync();
            var accountTask         = gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();
            var achievementsTask    = gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            var walletTask          = gw2ApiManager.Gw2ApiClient.V2.Account.Wallet.GetAsync();
            var progressionTask     = gw2ApiManager.Gw2ApiClient.V2.Account.Progression.GetAsync();
            var bankTask            = gw2ApiManager.Gw2ApiClient.V2.Account.Bank.GetAsync();
            var sharedInventoryTask = gw2ApiManager.Gw2ApiClient.V2.Account.Inventory.GetAsync();
            var materialStorageTask = gw2ApiManager.Gw2ApiClient.V2.Account.Materials.GetAsync();

            var apiResponseTasks = new List<Task>
            {
                charactersTask,
                pvpStatsTask,
                accountTask,
                achievementsTask,
                walletTask,
                progressionTask,
                bankTask,
                sharedInventoryTask,
                materialStorageTask
            };

            await Task.WhenAll(apiResponseTasks);

            model.GetEntry(EntryId.WVW_RANK).Value.Total = accountTask.Result.WvwRank ?? 0;
            OtherTotalValueService.SetDeathsTotalValue(model, charactersTask);
            OtherTotalValueService.SetLuckTotalValue(model, progressionTask);
            OtherTotalValueService.SetPvpTotalValues(model, pvpStatsTask);
            CurrencyTotalValueService.SetCurrencyTotalValues(model, walletTask);
            AchievementTotalValueService.SetAchievementTotalValues(model, achievementsTask);
            ItemSearchService.SetItemTotalValues(model, charactersTask, bankTask, sharedInventoryTask, materialStorageTask);

            model.UiHasToBeUpdated = true;
        }

        public static List<TokenPermission> API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE => new List<TokenPermission>
        {
            TokenPermission.Account,
            TokenPermission.Characters,
            TokenPermission.Progression,
            TokenPermission.Wallet,
            TokenPermission.Unlocks,
            TokenPermission.Pvp,
            TokenPermission.Inventories,
        };

        private static List<TokenPermission> API_TOKEN_PERMISSIONS_EVERY_API_KEY_HAS_BY_DEFAULT => new List<TokenPermission>
        {
            TokenPermission.Account
        };
    }
}