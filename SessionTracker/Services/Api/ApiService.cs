using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json.Linq;
using SessionTracker.Models;
using SessionTracker.Models.Constants;
using SessionTracker.Settings;

namespace SessionTracker.Services.Api
{
    public class ApiService
    {
        /// <summary>
        /// This is a workaround until this bug in blish is fixed: when a new module version requires additional api permissions, it will not get those until the
        /// module is disabled and enabled again. Blish restarts or module reinstalls do not fix this issue.
        /// </summary>
        public static async Task<bool> IsApiTokenGeneratedWithoutRequiredPermissions(Logger logger)
        {
            try
            {
                var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify);
                var blishSettingsFilePath = Path.Combine(documentsFolderPath, @"Guild Wars 2\addons\blishhud\settings.json");
                using var fileStream = System.IO.File.OpenRead(blishSettingsFilePath);
                using var streamReader = new StreamReader(fileStream);
                var settingsFileContent = await streamReader.ReadToEndAsync();
                var userPermissionsJToken = JObject.Parse(settingsFileContent).SelectToken("$..['ecksofa.sessiontracker'].UserEnabledPermissions");
                var storedTokenPermissions = userPermissionsJToken?.ToObject<List<TokenPermission>>();
                return API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE.Any(t => !storedTokenPermissions.Contains(t));
            }
            catch (Exception e)
            {
                logger.Warn(e, "failed to read permissions from settings.json for blish permissions bug workaround");
                return false;
            }
        }

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

            try
            {
                await Task.WhenAll(apiResponseTasks);
            }
            catch (Exception e)
            {
                throw new LogWarnException("API error", e);
            }

            model.GetStat(StatId.WVW_RANK).Value.Total = accountTask.Result.WvwRank ?? 0;
            OtherTotalValueService.SetDeathsTotalValue(model, charactersTask);
            OtherTotalValueService.SetLuckTotalValue(model, progressionTask);
            OtherTotalValueService.SetPvpTotalValues(model, pvpStatsTask);
            CurrencyTotalValueService.SetCurrencyTotalValues(model, walletTask);
            AchievementTotalValueService.SetAchievementTotalValues(model, achievementsTask);
            ItemSearchService.SetItemTotalValues(model, charactersTask, bankTask, sharedInventoryTask, materialStorageTask);

            model.UiHasToBeUpdated = true;
        }

        public static IReadOnlyList<TokenPermission> API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE => new List<TokenPermission>
        {
            TokenPermission.Account,
            TokenPermission.Characters,
            TokenPermission.Progression,
            TokenPermission.Wallet,
            TokenPermission.Unlocks,
            TokenPermission.Pvp,
            TokenPermission.Inventories,
        };

        private static IReadOnlyList<TokenPermission> API_TOKEN_PERMISSIONS_EVERY_API_KEY_HAS_BY_DEFAULT => new List<TokenPermission>
        {
            TokenPermission.Account
        };
    }
}