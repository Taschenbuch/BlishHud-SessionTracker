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
    }
}