using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.Services
{
    public class ApiService
    {
        public static async Task UpdateTotalValuesInModel(Model model, Gw2ApiManager gw2ApiManager)
        {
            var charactersTask   = gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var accountTask      = gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();
            var achievementsTask = gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            var walletTask       = gw2ApiManager.Gw2ApiClient.V2.Account.Wallet.GetAsync();

            await Task.WhenAll(charactersTask, accountTask, achievementsTask, walletTask);

            model.GetEntry(EntryId.DEATHS).Value.Total   = charactersTask.Result.Sum(c => c.Deaths);
            model.GetEntry(EntryId.WVW_RANK).Value.Total = accountTask.Result.WvwRank ?? 0;

            foreach (var entry in model.Entries.Where(v => v.IsCurrency))
                entry.Value.Total = GetCurrencyValue(walletTask, entry.CurrencyId);


            foreach (var entry in model.Entries.Where(v => v.IsAchievement))
                entry.Value.Total = GetAchievementValue(achievementsTask, entry.AchievementId);
        }

        private static int GetCurrencyValue(Task<IApiV2ObjectList<AccountCurrency>> walletTask, int currencyId)
        {
            // not sure if value can be missing == null. 
            return walletTask.Result
                             .FirstOrDefault(c => c.Id == currencyId)
                             ?.Value ?? 0;
        }

        private static int GetAchievementValue(Task<IApiV2ObjectList<AccountAchievement>> achievementsTask, int achievementId)
        {
            // reason for FirstOrDefault: for achievements that have a value of 0, the achievement is missing in the api response.
            return achievementsTask.Result
                                   .FirstOrDefault(a => a.Id == achievementId)
                                   ?.Current ?? 0;
        }

        public static List<TokenPermission> NECESSARY_API_TOKEN_PERMISSIONS => new List<TokenPermission>
        {
            TokenPermission.Account,
            TokenPermission.Characters,
            TokenPermission.Progression,
            TokenPermission.Wallet,
        };
    }
}