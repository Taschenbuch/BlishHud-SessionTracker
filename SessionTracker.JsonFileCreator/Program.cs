using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        static async Task Main()
        {
            var currencyEntries = await CurrencyService.CreateCurrencyEntries();

            var model = new Model();
            model.Entries.AddRange(_manuallyCreatedEntries);
            model.Entries.AddRange(currencyEntries);

            var json = JsonConvert.SerializeObject(model, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(@"C:\gw2\session\model.json", json);
        }

        private static readonly List<Entry> _manuallyCreatedEntries = new List<Entry>()
        {
            new Entry { Id = EntryId.DEATHS, LabelText                = { English = "Deaths" }, LabelTooltip              = { English = "Deaths\nCombined deaths from all sources like WvW, sPvP, PvE" }, IconFileName = "death.png" },
            new Entry { Id = EntryId.PVP_KILLS, LabelText             = { English = "PvP kills" }, LabelTooltip           = { English = "PvP kills" }, AchievementId                                                   = 239, IconFileName = "kill.png" },
            new Entry { Id = EntryId.PVP_KDR, LabelText               = { English = "PvP k/d" }, LabelTooltip             = { English = "PvP kills/deaths ratio" }, IconFileName                                       = "kdr.png" },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, LabelText        = { English = "PvP total wins" }, LabelTooltip      = { English = "PvP total wins" }, IconFileName                                               = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, LabelText      = { English = "PvP total losses" }, LabelTooltip    = { English = "PvP total losses" }, IconFileName                                             = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, LabelText       = { English = "PvP ranked wins" }, LabelTooltip     = { English = "PvP ranked wins" }, IconFileName                                              = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, LabelText     = { English = "PvP ranked losses" }, LabelTooltip   = { English = "PvP ranked losses" }, IconFileName                                            = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, LabelText     = { English = "PvP unranked wins" }, LabelTooltip   = { English = "PvP unranked wins" }, IconFileName                                            = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, LabelText   = { English = "PvP unranked losses" }, LabelTooltip = { English = "PvP unranked losses" }, IconFileName                                          = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, LabelText       = { English = "PvP custom wins" }, LabelTooltip     = { English = "PvP custom wins" }, IconFileName                                              = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, LabelText     = { English = "PvP custom losses" }, LabelTooltip   = { English = "PvP custom losses" }, IconFileName                                            = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, LabelText              = { English = "PvP rank" }, LabelTooltip            = { English = "PvP rank" }, IconFileName                                                     = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, LabelText    = { English = "PvP ranking points" }, LabelTooltip  = { English = "PvP ranking points" }, IconFileName                                           = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, LabelText             = { English = "WvW kills" }, LabelTooltip           = { English = "WvW kills" }, AchievementId                                                   = 283, IconFileName = "kill.png" },
            new Entry { Id = EntryId.WVW_KDR, LabelText               = { English = "WvW k/d" }, LabelTooltip             = { English = "WvW kills/deaths ratio" }, IconFileName                                       = "kdr.png" },
            new Entry { Id = EntryId.WVW_RANK, LabelText              = { English = "WvW rank" }, LabelTooltip            = { English = "WvW rank" }, IconFileName                                                     = "wvwRank.png" },
            new Entry { Id = "wvw supply spent on repairs", LabelText = { English = "Supply (repair)" }, LabelTooltip     = { English = "Supply spent on repairs" }, AchievementId                                     = 306, IconFileName = "supplySpend.png" },
            new Entry { Id = "wvw dolyaks killed", LabelText          = { English = "Dolyaks killed" }, LabelTooltip      = { English = "Dolyaks killed" }, AchievementId                                              = 288, IconFileName = "dolyak.png" },
            new Entry { Id = "wvw dolyaks escorted", LabelText        = { English = "Dolyaks escorted" }, LabelTooltip    = { English = "Dolyaks escorted" }, AchievementId                                            = 285, IconFileName = "dolyakDefended.png" },
            new Entry { Id = "wvw camps captured", LabelText          = { English = "Camps captured" }, LabelTooltip      = { English = "Supply camps captured" }, AchievementId                                       = 291, IconFileName = "camp.png" },
            new Entry { Id = "wvw camps defended", LabelText          = { English = "Camps defended" }, LabelTooltip      = { English = "Supply camps defended" }, AchievementId                                       = 310, IconFileName = "campDefended.png" },
            new Entry { Id = "wvw towers captured", LabelText         = { English = "Towers captured" }, LabelTooltip     = { English = "Towers captured" }, AchievementId                                             = 297, IconFileName = "tower.png" },
            new Entry { Id = "wvw towers defended", LabelText         = { English = "Towers defended" }, LabelTooltip     = { English = "Towers defended" }, AchievementId                                             = 322, IconFileName = "towerDefended.png" },
            new Entry { Id = "wvw keeps captured", LabelText          = { English = "Keeps captured" }, LabelTooltip      = { English = "Keeps captured" }, AchievementId                                              = 300, IconFileName = "keep.png" },
            new Entry { Id = "wvw keeps defended", LabelText          = { English = "Keeps defended" }, LabelTooltip      = { English = "Keeps defended" }, AchievementId                                              = 316, IconFileName = "keepDefended.png" },
            new Entry { Id = "wvw castles captured", LabelText        = { English = "Castles captured" }, LabelTooltip    = { English = "Stonemist castles captured" }, AchievementId                                  = 294, IconFileName = "castle.png" },
            new Entry { Id = "wvw castles defended", LabelText        = { English = "Castles defended" }, LabelTooltip    = { English = "Stonemist castles defended" }, AchievementId                                  = 313, IconFileName = "castleDefended.png" },
            new Entry { Id = "wvw objectives captured", LabelText     = { English = "Objectives captured" }, LabelTooltip = { English = "Objectives captured" }, AchievementId                                         = 303, IconFileName = "objective.png" },
            new Entry { Id = "wvw objectives defended", LabelText     = { English = "Objectives defended" }, LabelTooltip = { English = "Objectives defended" }, AchievementId                                         = 319, IconFileName = "objectiveDefended.png" },
        };
    }
}