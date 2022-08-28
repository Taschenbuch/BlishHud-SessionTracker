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
            SetCustomLabelTooltipForSomeStats(_manuallyCreatedStats);

            return _manuallyCreatedStats;
        }

        private static void SetCustomLabelTooltipForSomeStats(List<Entry> manuallyCreatedStats)
        {
            SetTooltip(
                manuallyCreatedStats,
                EntryId.DEATHS,
                "Combined deaths from all sources (WvW, sPvP, PvE)",
                "Aufsummierte Tode aus allen Quellen (WvW, sPvP, PvE)",
                "Nombre total de morts venant de toutes les sources (JcE, JcJ, McM)",
                "Número total de muertes de todas las fuentes (McM, JcJ, JcE)");

            SetTooltip(
                manuallyCreatedStats,
                EntryId.PVP_KDR,
                "PvP kills/deaths ratio",
                "PvP Verhältnis besiegte Feinde/Tode",
                "Ratio Victimes/Morts JcJ",
                "PvP Ratio de Bajas/Muertes");

            SetTooltip(
                manuallyCreatedStats,
                EntryId.WVW_KDR,
                "WvW kills/deaths ratio",
                "WvW Verhältnis besiegte Feinde/Tode",
                "Ratio Victimes/Morts en McM",
                "McM Ratio de Bajas/Muertes");

            SetTooltip(
                manuallyCreatedStats,
                EntryId.WVW_SUPPLY_REPAIR,
                "Supply spent on repairs",
                "Durch Reparieren verbrauchte Vorräte",
                "Quantité de ravitaillement utilisé pour les réparations",
                "Suministros gastados en reparaciones");
        }

        private static void SetTooltip(List<Entry> manuallyCreatedStats, string entryId, string english, string german, string french, string spanish)
        {
            var entry = manuallyCreatedStats.Single(s => s.Id == entryId);
            entry.LabelTooltip.LocalizedTextByLocale[Locale.English] = english;
            entry.LabelTooltip.LocalizedTextByLocale[Locale.German]  = german;
            entry.LabelTooltip.LocalizedTextByLocale[Locale.French]  = french;
            entry.LabelTooltip.LocalizedTextByLocale[Locale.Spanish] = spanish;
        }

        private static readonly List<Entry> _manuallyCreatedStats = new List<Entry>
        {
            new Entry { Id = EntryId.LUCK, LabelText                = { LocalizedTextByLocale = { [Locale.English] = "Luck", [Locale.German]                = "Glück", [Locale.French]                         = "Chance", [Locale.Spanish]                       = "Suerte" } }, IconFileName                           = "luck.png" },
            new Entry { Id = EntryId.DEATHS, LabelText              = { LocalizedTextByLocale = { [Locale.English] = "Deaths", [Locale.German]              = "Tode", [Locale.French]                          = "Morts", [Locale.Spanish]                        = "Muertes" } }, IconFileName                          = "death.png" },
            new Entry { Id = EntryId.PVP_KILLS, LabelText           = { LocalizedTextByLocale = { [Locale.English] = "PvP kills", [Locale.German]           = "PvP Feinde besiegt", [Locale.French]            = "Victimes JcJ", [Locale.Spanish]                 = "PvP bajas" } }, AchievementId                       = 239, IconFileName = "kill.png" },
            new Entry { Id = EntryId.PVP_KDR, LabelText             = { LocalizedTextByLocale = { [Locale.English] = "PvP KDR", [Locale.German]             = "PvP KDR", [Locale.French]                       = "KDR JcJ", [Locale.Spanish]                      = "PvP B/M" } }, IconFileName                          = "kdr.png" },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, LabelText      = { LocalizedTextByLocale = { [Locale.English] = "PvP total wins", [Locale.German]      = "PvP gesamt gewonnen", [Locale.French]           = "Victoires JcJ", [Locale.Spanish]                = "PvP Victorias totales" } }, IconFileName            = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, LabelText    = { LocalizedTextByLocale = { [Locale.English] = "PvP total losses", [Locale.German]    = "PvP gesamt verloren", [Locale.French]           = "Défaites JcJ", [Locale.Spanish]                 = "PvP Derrotas totales" } }, IconFileName             = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, LabelText     = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked wins", [Locale.German]     = "PvP mit rangwertung gewonnen", [Locale.French]  = "Victoires JcJ classées", [Locale.Spanish]       = "PvP Victorias en clasificatorias" } }, IconFileName = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked losses", [Locale.German]   = "PvP mit rangwertung verloren", [Locale.French]  = "Défaites JcJ classées", [Locale.Spanish]        = "PvP Derrotas en clasificatorias" } }, IconFileName  = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked wins", [Locale.German]   = "PvP ohne rangwertung gewonnen", [Locale.French] = "Victoires JcJ non classées", [Locale.Spanish]   = "PvP Victorias en libres" } }, IconFileName          = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, LabelText = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked losses", [Locale.German] = "PvP ohne rangwertung verloren", [Locale.French] = "Défaites JcJ non classées", [Locale.Spanish]    = "PvP Derrotas en libres" } }, IconFileName           = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, LabelText     = { LocalizedTextByLocale = { [Locale.English] = "PvP custom wins", [Locale.German]     = "Pvp selbsterstellt gewonnen", [Locale.French]   = "Victoires JcJ personnalisé", [Locale.Spanish]   = "PvP Victorias en personalidadas" } }, IconFileName  = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "PvP custom losses", [Locale.German]   = "PvP selbsterstellt verloren", [Locale.French]   = "Défaites JcJ personnalisé", [Locale.Spanish]    = "PvP Derrotas en personalidadas" } }, IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, LabelText            = { LocalizedTextByLocale = { [Locale.English] = "PvP rank", [Locale.German]            = "PvP rang", [Locale.French]                      = "Rang JcJ", [Locale.Spanish]                     = "PvP Rango" } }, IconFileName                        = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, LabelText  = { LocalizedTextByLocale = { [Locale.English] = "PvP ranking points", [Locale.German]  = "PvP rang punkte", [Locale.French]               = "Points de classement JcJ", [Locale.Spanish]     = "PvP Puntos de clasificación" } }, IconFileName      = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, LabelText           = { LocalizedTextByLocale = { [Locale.English] = "WvW kills", [Locale.German]           = "WvW Feinde besiegt", [Locale.French]            = "Victimes McM", [Locale.Spanish]                 = "McM Bajas" } }, AchievementId                       = 283, IconFileName = "kill.png" },
            new Entry { Id = EntryId.WVW_KDR, LabelText             = { LocalizedTextByLocale = { [Locale.English] = "WvW KDR", [Locale.German]             = "WvW KDR", [Locale.French]                       = "KDR McM", [Locale.Spanish]                      = "McM B/M" } }, IconFileName                          = "kdr.png" },
            new Entry { Id = EntryId.WVW_RANK, LabelText            = { LocalizedTextByLocale = { [Locale.English] = "WvW rank", [Locale.German]            = "WvW rang", [Locale.French]                      = "Rang McM", [Locale.Spanish]                     = "McM Rango" } }, IconFileName                        = "wvwRank.png" },
            new Entry { Id = EntryId.WVW_SUPPLY_REPAIR, LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Supply (repair)", [Locale.German]     = "Vorräte (reparieren)", [Locale.French]          = "Ravitaillement (réparations)", [Locale.Spanish] = "Suministros (reparaciones)" } }, AchievementId      = 306, IconFileName = "supplySpend.png" },
            new Entry { Id = "wvw dolyaks killed", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks killed", [Locale.German]      = "Karawane besiegt", [Locale.French]              = "Caravanes tuées", [Locale.Spanish]              = "Caravanas asesinadas" } }, AchievementId            = 288, IconFileName = "dolyak.png" },
            new Entry { Id = "wvw dolyaks escorted", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks escorted", [Locale.German]    = "Karawane verteidgt", [Locale.French]            = "Caravanes escortées", [Locale.Spanish]          = "Caravanas escoltadas" } }, AchievementId            = 285, IconFileName = "dolyakDefended.png" },
            new Entry { Id = "wvw camps captured", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Camps captured", [Locale.German]      = "Lager erobert", [Locale.French]                 = "Camps capturés", [Locale.Spanish]               = "Campamentos capturados" } }, AchievementId          = 291, IconFileName = "camp.png" },
            new Entry { Id = "wvw camps defended", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Camps defended", [Locale.German]      = "Lager verteidigt", [Locale.French]              = "Camps défendus", [Locale.Spanish]               = "Campamentos defendidos" } }, AchievementId          = 310, IconFileName = "campDefended.png" },
            new Entry { Id = "wvw towers captured", LabelText       = { LocalizedTextByLocale = { [Locale.English] = "Towers captured", [Locale.German]     = "Türme erobert", [Locale.French]                 = "Tours capturées", [Locale.Spanish]              = "Torres capturadas" } }, AchievementId               = 297, IconFileName = "tower.png" },
            new Entry { Id = "wvw towers defended", LabelText       = { LocalizedTextByLocale = { [Locale.English] = "Towers defended", [Locale.German]     = "Türme verteidigt", [Locale.French]              = "Tours défendues", [Locale.Spanish]              = "Torres defendidas" } }, AchievementId               = 322, IconFileName = "towerDefended.png" },
            new Entry { Id = "wvw keeps captured", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Keeps captured", [Locale.German]      = "Festungen erobert", [Locale.French]             = "Forts capturés", [Locale.Spanish]               = "Fortalezas capturadas" } }, AchievementId           = 300, IconFileName = "keep.png" },
            new Entry { Id = "wvw keeps defended", LabelText        = { LocalizedTextByLocale = { [Locale.English] = "Keeps defended", [Locale.German]      = "Festungen verteidgt", [Locale.French]           = "Forts défendus", [Locale.Spanish]               = "Fortalezas defendidas" } }, AchievementId           = 316, IconFileName = "keepDefended.png" },
            new Entry { Id = "wvw castles captured", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Castles captured", [Locale.German]    = "Schloss erobert", [Locale.French]               = "Châteaux capturés", [Locale.Spanish]            = "Castillos capturados" } }, AchievementId            = 294, IconFileName = "castle.png" },
            new Entry { Id = "wvw castles defended", LabelText      = { LocalizedTextByLocale = { [Locale.English] = "Castles defended", [Locale.German]    = "Schloss verteidgt", [Locale.French]             = "Châteaux défendus", [Locale.Spanish]            = "Castillos defendidos" } }, AchievementId            = 313, IconFileName = "castleDefended.png" },
            new Entry { Id = "wvw objectives captured", LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Objectives captured", [Locale.German] = "Objekte erobert", [Locale.French]               = "Objectifs capturés", [Locale.Spanish]           = "Objetivos capturados" } }, AchievementId            = 303, IconFileName = "objective.png" },
            new Entry { Id = "wvw objectives defended", LabelText   = { LocalizedTextByLocale = { [Locale.English] = "Objectives defended", [Locale.German] = "Objekte verteidigt", [Locale.French]            = "Objectifs défendus", [Locale.Spanish]           = "Objetivos defendidos" } }, AchievementId            = 319, IconFileName = "objectiveDefended.png" },
        };
    }
}