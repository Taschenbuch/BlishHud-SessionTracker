using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    public class ManuallyCreatedStatsService
    {
        public static IEnumerable<Entry> CreateManuallyCreatedStats()
        {
            SetAllLabelTooltipsToLabelText(_manuallyCreatedStats);
            SetCustomTooltipForSomeStats(_manuallyCreatedStats);

            return _manuallyCreatedStats;
        }

        private static void SetAllLabelTooltipsToLabelText(List<Entry> manuallyCreatedStats)
        {
            foreach (var manuallyCreatedStat in manuallyCreatedStats)
                manuallyCreatedStat.LabelTooltip.English = manuallyCreatedStat.LabelText.English;
        }

        private static void SetCustomTooltipForSomeStats(List<Entry> manuallyCreatedStats)
        {
            manuallyCreatedStats.Single(s => s.Id == EntryId.DEATHS).LabelTooltip.English            = "Deaths\nCombined deaths from all sources like WvW, sPvP, PvE";
            manuallyCreatedStats.Single(s => s.Id == EntryId.PVP_KDR).LabelTooltip.English           = "PvP kills/deaths ratio";
            manuallyCreatedStats.Single(s => s.Id == EntryId.WVW_KDR).LabelTooltip.English           = "WvW kills/deaths ratio";
            manuallyCreatedStats.Single(s => s.Id == EntryId.WVW_SUPPLY_REPAIR).LabelTooltip.English = "Supply spent on repairs";
        }

        private static readonly List<Entry> _manuallyCreatedStats = new List<Entry>
        {
            new Entry { Id = EntryId.DEATHS, LabelText              = { English = "Deaths" }, IconFileName               = "death.png" },
            new Entry { Id = EntryId.PVP_KILLS, LabelText           = { English = "PvP kills" }, AchievementId           = 239, IconFileName = "kill.png" },
            new Entry { Id = EntryId.PVP_KDR, LabelText             = { English = "PvP k/d" }, IconFileName              = "kdr.png" },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, LabelText      = { English = "PvP total wins" }, IconFileName       = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, LabelText    = { English = "PvP total losses" }, IconFileName     = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, LabelText     = { English = "PvP ranked wins" }, IconFileName      = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, LabelText   = { English = "PvP ranked losses" }, IconFileName    = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, LabelText   = { English = "PvP unranked wins" }, IconFileName    = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, LabelText = { English = "PvP unranked losses" }, IconFileName  = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, LabelText     = { English = "PvP custom wins" }, IconFileName      = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, LabelText   = { English = "PvP custom losses" }, IconFileName    = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, LabelText            = { English = "PvP rank" }, IconFileName             = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, LabelText  = { English = "PvP ranking points" }, IconFileName   = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, LabelText           = { English = "WvW kills" }, AchievementId           = 283, IconFileName = "kill.png" },
            new Entry { Id = EntryId.WVW_KDR, LabelText             = { English = "WvW k/d" }, IconFileName              = "kdr.png" },
            new Entry { Id = EntryId.WVW_RANK, LabelText            = { English = "WvW rank" }, IconFileName             = "wvwRank.png" },
            new Entry { Id = EntryId.WVW_SUPPLY_REPAIR, LabelText   = { English = "Supply (repair)" }, AchievementId     = 306, IconFileName = "supplySpend.png" },
            new Entry { Id = "wvw dolyaks killed", LabelText        = { English = "Dolyaks killed" }, AchievementId      = 288, IconFileName = "dolyak.png" },
            new Entry { Id = "wvw dolyaks escorted", LabelText      = { English = "Dolyaks escorted" }, AchievementId    = 285, IconFileName = "dolyakDefended.png" },
            new Entry { Id = "wvw camps captured", LabelText        = { English = "Camps captured" }, AchievementId      = 291, IconFileName = "camp.png" },
            new Entry { Id = "wvw camps defended", LabelText        = { English = "Camps defended" }, AchievementId      = 310, IconFileName = "campDefended.png" },
            new Entry { Id = "wvw towers captured", LabelText       = { English = "Towers captured" }, AchievementId     = 297, IconFileName = "tower.png" },
            new Entry { Id = "wvw towers defended", LabelText       = { English = "Towers defended" }, AchievementId     = 322, IconFileName = "towerDefended.png" },
            new Entry { Id = "wvw keeps captured", LabelText        = { English = "Keeps captured" }, AchievementId      = 300, IconFileName = "keep.png" },
            new Entry { Id = "wvw keeps defended", LabelText        = { English = "Keeps defended" }, AchievementId      = 316, IconFileName = "keepDefended.png" },
            new Entry { Id = "wvw castles captured", LabelText      = { English = "Castles captured" }, AchievementId    = 294, IconFileName = "castle.png" },
            new Entry { Id = "wvw castles defended", LabelText      = { English = "Castles defended" }, AchievementId    = 313, IconFileName = "castleDefended.png" },
            new Entry { Id = "wvw objectives captured", LabelText   = { English = "Objectives captured" }, AchievementId = 303, IconFileName = "objective.png" },
            new Entry { Id = "wvw objectives defended", LabelText   = { English = "Objectives defended" }, AchievementId = 319, IconFileName = "objectiveDefended.png" },
        };
    }
}