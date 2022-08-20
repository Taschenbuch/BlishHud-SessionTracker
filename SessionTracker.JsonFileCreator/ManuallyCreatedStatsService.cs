using System.Collections.Generic;
using System.Linq;
using Gw2Sharp.WebApi;
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
            {
                manuallyCreatedStat.LabelTooltip.LocalizedTextByLocale.Clear();

                foreach (var labelTextAndLocale in manuallyCreatedStat.LabelText.LocalizedTextByLocale)
                   manuallyCreatedStat.LabelTooltip.LocalizedTextByLocale[labelTextAndLocale.Key] = labelTextAndLocale.Value;
            }
        }

        private static void SetCustomTooltipForSomeStats(List<Entry> manuallyCreatedStats)
        {
            // only english for now
            manuallyCreatedStats.Single(s => s.Id == EntryId.DEATHS).LabelTooltip.English            = "Deaths\nCombined deaths from all sources like WvW, sPvP, PvE";
            manuallyCreatedStats.Single(s => s.Id == EntryId.PVP_KDR).LabelTooltip.English           = "PvP kills/deaths ratio";
            manuallyCreatedStats.Single(s => s.Id == EntryId.WVW_KDR).LabelTooltip.English           = "WvW kills/deaths ratio";
            manuallyCreatedStats.Single(s => s.Id == EntryId.WVW_SUPPLY_REPAIR).LabelTooltip.English = "Supply spent on repairs";
        }

        private static readonly List<Entry> _manuallyCreatedStats = new List<Entry>
        {
            new Entry { Id = EntryId.DEATHS, LabelText              = { LocalizedTextByLocale = { [Locale.English] = "Deaths", [Locale.German]              = "Tode", [Locale.French]                          = "Morts", [Locale.Spanish]                       = "Muertes" } }, IconFileName                      = "death.png" },
            new Entry { Id = EntryId.PVP_KILLS, LabelText           = { LocalizedTextByLocale = { [Locale.English] = "PvP kills", [Locale.German]           = "PvP Feinde besiegt", [Locale.French]            = "JcJ victimes", [Locale.Spanish]                = "PvP eliminados" } }, AchievementId              = 239, IconFileName = "kill.png" },
            new Entry { Id = EntryId.PVP_KDR, LabelText             = { LocalizedTextByLocale = { [Locale.English] = "PvP k/d", [Locale.German]             = "PvP b/t", [Locale.French]                       = "JcJ v/m", [Locale.Spanish]                     = "PvP e/m" } }, IconFileName                      = "kdr.png" },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, LabelText      = { LocalizedTextByLocale = { [Locale.English] = "PvP total wins", [Locale.German]      = "PvP gesamt gewonnen", [Locale.French]           = "JcJ total remporte", [Locale.Spanish]          = "PvP total victorias" } }, IconFileName          = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, LabelText    = { LocalizedTextByLocale = { [Locale.English] = "PvP total losses", [Locale.German]    = "PvP gesamt verloren", [Locale.French]           = "JcJ total pertes", [Locale.Spanish]            = "PvP total perdido" } }, IconFileName            = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, LabelText     = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked wins", [Locale.German]     = "PvP mit rangwertung gewonnen", [Locale.French]  = "JcJ classée remporte", [Locale.Spanish]        = "PvP clasificatoria victorias" } }, IconFileName = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked losses", [Locale.German]   = "PvP mit rangwertung verloren", [Locale.French]  = "JcJ classée pertes", [Locale.Spanish]          = "PvP clasificatoria perdido" } }, IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked wins", [Locale.German]   = "PvP ohne rangwertung gewonnen", [Locale.French] = "JcJ non classée remporte", [Locale.Spanish]    = "PvP libre victorias" } }, IconFileName          = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, LabelText = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked losses", [Locale.German] = "PvP ohne rangwertung verloren", [Locale.French] = "JcJ non classée pertes", [Locale.Spanish]      = "PvP libre perdido" } }, IconFileName            = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, LabelText     = { LocalizedTextByLocale = { [Locale.English] = "PvP custom wins", [Locale.German]     = "Pvp selbsterstellt gewonnen", [Locale.French]   = "JcJ personnalisées remporte", [Locale.Spanish] = "PvP personalizadas victorias" } }, IconFileName = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP custom losses", [Locale.German]   = "PvP selbsterstellt verloren", [Locale.French]   = "JcJ personnalisées pertes", [Locale.Spanish]   = "PvP personalizadas perdido" } }, IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, LabelText            = { LocalizedTextByLocale = { [Locale.English] = "PvP rank", [Locale.German]            = "PvP rang", [Locale.French]                      = "JcJ rang", [Locale.Spanish]                    = "PvP rango" } }, IconFileName                    = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, LabelText  = { LocalizedTextByLocale = { [Locale.English] = "PvP ranking points", [Locale.German]  = "PvP rang punkte", [Locale.French]               = "JcJ points de classement", [Locale.Spanish]    = "PvP puntos de clasificación" } }, IconFileName  = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, LabelText           = { LocalizedTextByLocale = { [Locale.English] = "WvW kills", [Locale.German]           = "WvW Feinde besiegt", [Locale.French]            = "McM victimes", [Locale.Spanish]                = "McM eliminados" } }, AchievementId              = 283, IconFileName = "kill.png" },
            new Entry { Id = EntryId.WVW_KDR, LabelText             = { LocalizedTextByLocale = { [Locale.English] = "WvW k/d", [Locale.German]             = "WvW b/t", [Locale.French]                       = "McM v/m", [Locale.Spanish]                     = "McM e/m" } }, IconFileName                      = "kdr.png" },
            new Entry { Id = EntryId.WVW_RANK, LabelText            = { LocalizedTextByLocale = { [Locale.English] = "WvW rank", [Locale.German]            = "WvW rang", [Locale.French]                      = "McM rang", [Locale.Spanish]                    = "McM rango" } }, IconFileName                    = "wvwRank.png" },
            new Entry { Id = EntryId.WVW_SUPPLY_REPAIR, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Supply (repair)", [Locale.German]     = "Vorräte (reparieren)", [Locale.French]          = "Ravitaillement (réparation)", [Locale.Spanish] = "Suministros (reparaciones)" } }, AchievementId  = 306, IconFileName = "supplySpend.png" },
            new Entry { Id = "wvw dolyaks killed", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks killed", [Locale.German]      = "Karawane besiegt", [Locale.French]              = "Caravane tuée", [Locale.Spanish]               = "Caravanas asesinadas" } }, AchievementId        = 288, IconFileName = "dolyak.png" },
            new Entry { Id = "wvw dolyaks escorted", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks escorted", [Locale.German]    = "Karawane verteidgt", [Locale.French]            = "Caravane escortée", [Locale.Spanish]           = "Caravanas escoltadas" } }, AchievementId        = 285, IconFileName = "dolyakDefended.png" },
            new Entry { Id = "wvw camps captured", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Camps captured", [Locale.German]      = "Lager erobert", [Locale.French]                 = "Camps capturés", [Locale.Spanish]              = "Campamentos capturados" } }, AchievementId      = 291, IconFileName = "camp.png" },
            new Entry { Id = "wvw camps defended", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Camps defended", [Locale.German]      = "Lager verteidigt", [Locale.French]              = "Camps défendus", [Locale.Spanish]              = "Campamentos defendidos" } }, AchievementId      = 310, IconFileName = "campDefended.png" },
            new Entry { Id = "wvw towers captured", LabelText       = { LocalizedTextByLocale = { [Locale.English] = "Towers captured", [Locale.German]     = "Türme erobert", [Locale.French]                 = "Tours capturées", [Locale.Spanish]             = "Torres capturadas" } }, AchievementId           = 297, IconFileName = "tower.png" },
            new Entry { Id = "wvw towers defended", LabelText       = { LocalizedTextByLocale = { [Locale.English] = "Towers defended", [Locale.German]     = "Türme verteidigt", [Locale.French]              = "Tours défendues", [Locale.Spanish]             = "Torres defendidas" } }, AchievementId           = 322, IconFileName = "towerDefended.png" },
            new Entry { Id = "wvw keeps captured", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Keeps captured", [Locale.German]      = "Festungen erobert", [Locale.French]             = "Fort capturé", [Locale.Spanish]                = "Fortalezas capturadas" } }, AchievementId       = 300, IconFileName = "keep.png" },
            new Entry { Id = "wvw keeps defended", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Keeps defended", [Locale.German]      = "Festungen verteidgt", [Locale.French]           = "Fort défendu", [Locale.Spanish]                = "Fortalezas defendidas" } }, AchievementId       = 316, IconFileName = "keepDefended.png" },
            new Entry { Id = "wvw castles captured", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Castles captured", [Locale.German]    = "Schloss erobert", [Locale.French]               = "Châteaux capturés", [Locale.Spanish]           = "Castillos capturados" } }, AchievementId        = 294, IconFileName = "castle.png" },
            new Entry { Id = "wvw castles defended", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Castles defended", [Locale.German]    = "Schloss verteidgt", [Locale.French]             = "Châteaux défendus", [Locale.Spanish]           = "Castillos defendidos" } }, AchievementId        = 313, IconFileName = "castleDefended.png" },
            new Entry { Id = "wvw objectives captured", LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Objectives captured", [Locale.German] = "Objekte erobert", [Locale.French]               = "Objectifs capturés", [Locale.Spanish]          = "Objetivos captados" } }, AchievementId          = 303, IconFileName = "objective.png" },
            new Entry { Id = "wvw objectives defended", LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Objectives defended", [Locale.German] = "Objekte verteidigt", [Locale.French]            = "Objectifs défendus", [Locale.Spanish]          = "Objetivos defendidos" } }, AchievementId        = 319, IconFileName = "objectiveDefended.png" },
        };
    }
}