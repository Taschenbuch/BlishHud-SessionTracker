using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json.Linq;
using SessionTracker.Constants;
using SessionTracker.Files.RemoteFiles;
using SessionTracker.Other;
using SessionTracker.OtherServices;

namespace SessionTracker.Api
{
    public class ApiService
    {
        /// <summary>
        /// This is a workaround until this bug in blish is fixed: when a new module version requires additional api permissions, it will not get those until the
        /// module is disabled and enabled again. Blish restarts or module reinstalls do not fix this issue.
        /// </summary>
        public static async Task<bool> IsApiTokenGeneratedWithoutRequiredPermissions(DirectoriesManager directoriesManager)
        {
            try
            {
                var moduleFolderPath = directoriesManager.GetFullDirectoryPath(FileConstants.MODULE_FOLDER_NAME);
                var blishSettingsFilePath = Path.Combine(moduleFolderPath, "..", "settings.json");
                using var fileStream = System.IO.File.OpenRead(blishSettingsFilePath);
                using var streamReader = new StreamReader(fileStream);
                var settingsFileContent = await streamReader.ReadToEndAsync();
                var userPermissionsJToken = JObject.Parse(settingsFileContent).SelectToken("$..['ecksofa.sessiontracker'].UserEnabledPermissions");
                var storedTokenPermissions = userPermissionsJToken?.ToObject<List<TokenPermission>>();
                // NullReferenceException exception when fresh blish start and not settings.json exists yet. But that is fine.
                return API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE.Any(t => !storedTokenPermissions.Contains(t)); 
            }
            catch (Exception e)
            {
                Module.Logger.Warn(e, "failed to read permissions from settings.json for blish permissions bug workaround.");
                return false;
            }
        }

        public static async Task UpdateTotalValuesInModel(Services services)
        {
            var charactersTask      = services.Gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var pvpStatsTask        = services.Gw2ApiManager.Gw2ApiClient.V2.Pvp.Stats.GetAsync();
            var accountTask         = services.Gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();
            var achievementsTask    = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            var walletTask          = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Wallet.GetAsync();
            var progressionTask     = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Progression.GetAsync();
            var bankTask            = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Bank.GetAsync();
            var sharedInventoryTask = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Inventory.GetAsync();
            var materialStorageTask = services.Gw2ApiManager.Gw2ApiClient.V2.Account.Materials.GetAsync();

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

            services.Model.GetStat(StatId.WVW_RANK).Value.Total = accountTask.Result.WvwRank ?? 0;
            OtherTotalValueService.SetDeathsTotalValue(services.Model, charactersTask);
            OtherTotalValueService.SetLuckTotalValue(services.Model, progressionTask);
            OtherTotalValueService.SetPvpTotalValues(services.Model, pvpStatsTask);
            CurrencyTotalValueService.SetCurrencyTotalValues(services.Model, walletTask);
            AchievementTotalValueService.SetAchievementTotalValues(services.Model, achievementsTask);
            ItemSearchService.SetItemTotalValues(services.Model, charactersTask, bankTask, sharedInventoryTask, materialStorageTask);
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
    }
}