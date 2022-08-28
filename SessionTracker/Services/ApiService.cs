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
        public static List<TokenPermission> ACCOUNT_API_TOKEN_PERMISSION => new List<TokenPermission>
        {
            TokenPermission.Account
        };

        public static List<TokenPermission> NECESSARY_API_TOKEN_PERMISSIONS => new List<TokenPermission>
        {
            TokenPermission.Account,
            TokenPermission.Characters,
            TokenPermission.Progression,
            TokenPermission.Wallet,
            TokenPermission.Unlocks,
            TokenPermission.Pvp,
        };

        public static async Task UpdateTotalValuesInModel(Model model, Gw2ApiManager gw2ApiManager)
        {
            var charactersTask   = gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var pvpStatsTask     = gw2ApiManager.Gw2ApiClient.V2.Pvp.Stats.GetAsync();
            var accountTask      = gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();
            var achievementsTask = gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            var walletTask       = gw2ApiManager.Gw2ApiClient.V2.Account.Wallet.GetAsync();
            var progressionTask  = gw2ApiManager.Gw2ApiClient.V2.Account.Progression.GetAsync();

            await Task.WhenAll(charactersTask, accountTask, achievementsTask, walletTask, progressionTask);

            model.GetEntry(EntryId.DEATHS).Value.Total   = charactersTask.Result.Sum(c => c.Deaths);
            model.GetEntry(EntryId.WVW_RANK).Value.Total = accountTask.Result.WvwRank ?? 0;
            
            SetLuckTotalValue(model, progressionTask);
            SetPvpTotalValues(model, pvpStatsTask);
            SetCurrencyTotalValues(model, walletTask);
            SetAchievementTotalValues(model, achievementsTask);

            model.UiHasToBeUpdated = true;
        }

        private static void SetAchievementTotalValues(Model model, Task<IApiV2ObjectList<AccountAchievement>> achievementsTask)
        {
            foreach (var entry in model.Entries.Where(v => v.IsAchievement))
                entry.Value.Total = GetAchievementValue(achievementsTask, entry.AchievementId);
        }

        private static void SetCurrencyTotalValues(Model model, Task<IApiV2ObjectList<AccountCurrency>> walletTask)
        {
            foreach (var entry in model.Entries.Where(v => v.IsCurrency))
                entry.Value.Total = GetCurrencyValue(walletTask, entry.CurrencyId);
        }

        private static void SetLuckTotalValue(Model model, Task<IApiV2ObjectList<AccountProgression>> progressionTask)
        {
            var luck        = progressionTask.Result.SingleOrDefault(p => p.Id == "luck");
            var hasZeroLuck = luck == null;
            model.GetEntry(EntryId.LUCK).Value.Total = hasZeroLuck ? 0 : luck.Value;
        }

        private static void SetPvpTotalValues(Model model, Task<PvpStats> pvpStatsTask)
        {
            var pvpRank          = pvpStatsTask.Result.PvpRank;
            var pvpRankRollovers = pvpStatsTask.Result.PvpRankRollovers;
            var totalWins        = pvpStatsTask.Result.Aggregate.Wins;
            var totalLosses      = pvpStatsTask.Result.Aggregate.Losses;
            var rankedWins       = pvpStatsTask.Result.Ladders["ranked"].Wins;
            var rankedLosses     = pvpStatsTask.Result.Ladders["ranked"].Losses;
            var unrankedWins     = pvpStatsTask.Result.Ladders["unranked"].Wins;
            var unrankedLosses   = pvpStatsTask.Result.Ladders["unranked"].Losses;

            model.GetEntry(EntryId.PVP_RANK).Value.Total            = pvpRank + pvpRankRollovers;
            model.GetEntry(EntryId.PVP_RANKING_POINTS).Value.Total  = pvpStatsTask.Result.PvpRankPoints;
            model.GetEntry(EntryId.PVP_TOTAL_WINS).Value.Total      = totalWins;
            model.GetEntry(EntryId.PVP_TOTAL_LOSSES).Value.Total    = totalLosses;
            model.GetEntry(EntryId.PVP_RANKED_WINS).Value.Total     = rankedWins;
            model.GetEntry(EntryId.PVP_RANKED_LOSSES).Value.Total   = rankedLosses;
            model.GetEntry(EntryId.PVP_UNRANKED_WINS).Value.Total   = unrankedWins;
            model.GetEntry(EntryId.PVP_UNRANKED_LOSSES).Value.Total = unrankedLosses;
            model.GetEntry(EntryId.PVP_CUSTOM_WINS).Value.Total     = totalWins - rankedWins - unrankedWins;
            model.GetEntry(EntryId.PVP_CUSTOM_LOSSES).Value.Total   = totalLosses - rankedLosses - unrankedLosses;
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
    }
}