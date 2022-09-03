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
            var pvpStatsTask     = gw2ApiManager.Gw2ApiClient.V2.Pvp.Stats.GetAsync();
            var achievementsTask = gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            var walletTask       = gw2ApiManager.Gw2ApiClient.V2.Account.Wallet.GetAsync();

            await Task.WhenAll(charactersTask, accountTask, achievementsTask, walletTask);

            var pvpRank          = pvpStatsTask.Result.PvpRank;
            var pvpRankRollovers = pvpStatsTask.Result.PvpRankRollovers;
            var totalWins        = pvpStatsTask.Result.Aggregate.Wins;
            var totalLosses      = pvpStatsTask.Result.Aggregate.Losses;

            var ladders = pvpStatsTask.Result.Ladders;
            var (rankedWins, rankedLosses)     = GetWinsAndLosses(ladders, RANKED_LADDERS_KEY);
            var (unrankedWins, unrankedLosses) = GetWinsAndLosses(ladders, UNRANKED_LADDERS_KEY);

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
            model.GetEntry(EntryId.DEATHS).Value.Total              = charactersTask.Result.Sum(c => c.Deaths);
            model.GetEntry(EntryId.WVW_RANK).Value.Total            = accountTask.Result.WvwRank ?? 0;

            foreach (var entry in model.Entries.Where(v => v.IsCurrency))
                entry.Value.Total = GetCurrencyValue(walletTask, entry.CurrencyId);

            foreach (var entry in model.Entries.Where(v => v.IsAchievement))
                entry.Value.Total = GetAchievementValue(achievementsTask, entry.AchievementId);

            model.UiHasToBeUpdated = true;
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
            TokenPermission.Pvp,
        };

        private static (int wins, int losses) GetWinsAndLosses(IReadOnlyDictionary<string, PvpStatsAggregate> ladders, string ladderKey)
        {
            // when no unranked or no ranked pvp games have been played by an account, it can happen that the unranked and/or ranked ladder object is missing.
            // A user had this issue. We checked their api response to confirm the issue.
            // On the other hand my test gw2 account has never played pvp either. And it did have the ranked and unranked objects in .Ladders with every value set to 0.
            // The reason why this is sometimes the case and sometimes not is currently unknown.
            if (ladders.ContainsKey(ladderKey) == false)
                return (0, 0);

            var ladder = ladders[ladderKey];
            return (ladder.Wins, ladder.Losses);
        }

        private const string RANKED_LADDERS_KEY = "ranked";
        private const string UNRANKED_LADDERS_KEY = "unranked";
    }
}