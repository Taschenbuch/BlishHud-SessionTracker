using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;
using SessionTracker.Models.Constants;

namespace SessionTracker.Services.Api
{
    public class OtherTotalValueService
    {
        public static void SetDeathsTotalValue(Model model, Task<IApiV2ObjectList<Character>> charactersTask)
        {
            var newDeaths = charactersTask.Result.Sum(c => c.Deaths);
            var oldDeaths = model.GetEntry(EntryId.DEATHS).Value.Total;
            model.GetEntry(EntryId.DEATHS).Value.Total = GetNewValueIfNotApiBug(newDeaths, oldDeaths);
        }

        // In rare cases the next total value is lower than the previous total value. It is an api bug that is visible in Gw2effiency graphs too.
        // The next api request typically gives the correct value again. So the issue is only temporary.
        // Happened with deaths (reported by users) and pvp/wvw kills (happened to me in the past, i think).
        public static int GetNewValueIfNotApiBug(int newValue, int oldValue)
        {
            var hastemporaryApiValueBug = newValue < oldValue;
            return hastemporaryApiValueBug
                ? oldValue
                : newValue;
        }

        public static void SetLuckTotalValue(Model model, Task<IApiV2ObjectList<AccountProgression>> progressionTask)
        {
            var luck        = progressionTask.Result.SingleOrDefault(p => p.Id == "luck");
            var hasZeroLuck = luck == null;
            model.GetEntry(EntryId.LUCK).Value.Total = hasZeroLuck ? 0 : luck.Value;
        }

        public static void SetPvpTotalValues(Model model, Task<PvpStats> pvpStatsTask)
        {
            var pvpRank          = pvpStatsTask.Result.PvpRank;
            var pvpRankRollovers = pvpStatsTask.Result.PvpRankRollovers;
            var totalWins        = pvpStatsTask.Result.Aggregate.Wins;
            var totalLosses      = pvpStatsTask.Result.Aggregate.Losses;
            var ladders          = pvpStatsTask.Result.Ladders;
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
        }

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