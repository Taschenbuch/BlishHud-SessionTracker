using System.Collections.Generic;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class WvwStatsCreator
    {
        public static List<Stat> CreateWvwStats()
        {
            var wvwStats = SetCategory(_wvwStats);
            wvwStats = StatsCreatorCommon.SetOrder(wvwStats);
            return wvwStats;
        }

        private static List<Stat> SetCategory(List<Stat> stats)
        {
            foreach (var stat in stats)
            {
                stat.Category.Name.LocalizedTextByLocale = new Dictionary<Locale, string>()
                {
                    { Locale.English, "Currencies" },
                    { Locale.French, "monnaies" },
                    { Locale.German, "Währungen" },
                    { Locale.Spanish, "divisas" },
                };
                stat.Category.Type = StatCategoryType.Wvw;
            }

            return stats;
        }

        private static readonly List<Stat> _wvwStats = new List<Stat>
        {
            new Stat
            {
                Id           = StatId.WVW_KDR,
                IconFileName = "kdr.png",
                Name         = { LocalizedTextByLocale = { [Locale.English] = "WvW KDR", [Locale.German]                = "WvW KDR", [Locale.French]                             = "KDR McM", [Locale.Spanish]                     = "McM B/M" } },
                Description  = { LocalizedTextByLocale = { [Locale.English] = "WvW kills/deaths ratio", [Locale.German] = "WvW Verhältnis besiegte Feinde/Tode", [Locale.French] = "Ratio Victimes/Morts en McM", [Locale.Spanish] = "McM Ratio de Bajas/Muertes" } }
            },
            new Stat
            {
                Id = StatId.WVW_RANK,
                IconFileName = "wvwRank.png",
                Name = { LocalizedTextByLocale = { [Locale.English] = "WvW rank", [Locale.German] = "WvW rang", [Locale.French] = "Rang McM", [Locale.Spanish] = "McM Rango" } },
            },
            new Stat
            {
                Id          = StatId.WVW_SUPPLY_REPAIR,
                ApiId       = 306,
                ApiIdType   = ApiIdType.Achievement,
                Name        = { LocalizedTextByLocale = { [Locale.English] = "Supply (repair)", [Locale.German]         = "Vorräte (reparieren)", [Locale.French]                 = "Ravitaillement (réparations)", [Locale.Spanish]                            = "Suministros (reparaciones)" } }, IconFileName = "supplySpend.png",
                Description = { LocalizedTextByLocale = { [Locale.English] = "Supply spent on repairs", [Locale.German] = "Durch Reparieren verbrauchte Vorräte", [Locale.French] = "Quantité de ravitaillement utilisé pour les réparations", [Locale.Spanish] = "Suministros gastados en reparaciones" } }
            },
            new Stat
            {
                Id = StatId.WVW_KILLS,
                IconFileName = "kill.png",
                ApiId = 283,
                ApiIdType = ApiIdType.Achievement,
                Name = { LocalizedTextByLocale = { [Locale.English] = "WvW kills", [Locale.German]           = "WvW Feinde besiegt", [Locale.French]            = "Victimes McM", [Locale.Spanish]               = "McM Bajas" } },
            },
            new Stat
            {
                Id   = "wvw dolyaks killed",
                IconFileName   = "dolyak.png",
                ApiId            = 288,
                ApiIdType = ApiIdType.Achievement,
                Name = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks killed", [Locale.German]      = "Karawane besiegt", [Locale.French]    = "Caravanes tuées", [Locale.Spanish]     = "Caravanas asesinadas" } },
            },
            new Stat 
            { 
                Id = "wvw dolyaks escorted",
                IconFileName   = "dolyakDefended.png", 
                ApiId    = 285,
                ApiIdType = ApiIdType.Achievement, 
                Name    = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks escorted", [Locale.German]    = "Karawane verteidgt", [Locale.French]  = "Caravanes escortées", [Locale.Spanish] = "Caravanas escoltadas" } }, 
            },
            new Stat 
            { 
                Id = "wvw camps captured",
                IconFileName = "camp.png", 
                ApiId              = 291,
                ApiIdType = ApiIdType.Achievement,
                Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps captured", [Locale.German]      = "Lager erobert", [Locale.French]       = "Camps capturés", [Locale.Spanish]      = "Campamentos capturados" } }, 
            },
            new Stat 
            { 
                Id = "wvw camps defended", 
                IconFileName = "campDefended.png", 
                ApiId      = 310,
                ApiIdType = ApiIdType.Achievement, 
                Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps defended", [Locale.German]      = "Lager verteidigt", [Locale.French]    = "Camps défendus", [Locale.Spanish]      = "Campamentos defendidos" } }, 
            },
            new Stat 
            { 
                Id = "wvw towers captured",
                IconFileName      = "tower.png",
                ApiId             = 297, 
                ApiIdType = ApiIdType.Achievement, 
                Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers captured", [Locale.German]     = "Türme erobert", [Locale.French]       = "Tours capturées", [Locale.Spanish]     = "Torres capturadas" } }, 
            },
            new Stat 
            { 
                Id = "wvw towers defended", 
                IconFileName      = "towerDefended.png", 
                ApiId     = 322, 
                ApiIdType = ApiIdType.Achievement, 
                Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers defended", [Locale.German]     = "Türme verteidigt", [Locale.French]    = "Tours défendues", [Locale.Spanish]     = "Torres defendidas" } }, 
            },
            new Stat 
            { 
                Id = "wvw keeps captured", 
                IconFileName  = "keep.png", 
                ApiId              = 300, 
                ApiIdType = ApiIdType.Achievement, 
                Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps captured", [Locale.German]      = "Festungen erobert", [Locale.French]   = "Forts capturés", [Locale.Spanish]      = "Fortalezas capturadas" } },
            },
            new Stat 
            { 
                Id = "wvw keeps defended", 
                IconFileName  = "keepDefended.png",
                ApiId      = 316, 
                ApiIdType = ApiIdType.Achievement, 
                Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps defended", [Locale.German]      = "Festungen verteidgt", [Locale.French] = "Forts défendus", [Locale.Spanish]      = "Fortalezas defendidas" } },
            },
            new Stat 
            { 
                Id = "wvw castles captured", 
                IconFileName   = "castle.png", 
                ApiId            = 294, 
                ApiIdType = ApiIdType.Achievement, 
                Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles captured", [Locale.German]    = "Schloss erobert", [Locale.French]     = "Châteaux capturés", [Locale.Spanish]   = "Castillos capturados" } }, 
            },
            new Stat 
            { 
                Id = "wvw castles defended", 
                IconFileName   = "castleDefended.png", 
                ApiId    = 313, 
                ApiIdType = ApiIdType.Achievement, 
                Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles defended", [Locale.German]    = "Schloss verteidgt", [Locale.French]   = "Châteaux défendus", [Locale.Spanish]   = "Castillos defendidos" } }, 
            },
            new Stat 
            { 
                Id = "wvw objectives captured", 
                IconFileName   = "objective.png", 
                ApiId         = 303, 
                ApiIdType = ApiIdType.Achievement, 
                Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives captured", [Locale.German] = "Objekte erobert", [Locale.French]     = "Objectifs capturés", [Locale.Spanish]  = "Objetivos capturados" } }, 
            },
            new Stat 
            {                
                Id = "wvw objectives defended", 
                IconFileName   = "objectiveDefended.png",
                ApiId = 319, 
                ApiIdType = ApiIdType.Achievement, 
                Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives defended", [Locale.German] = "Objekte verteidigt", [Locale.French]  = "Objectifs défendus", [Locale.Spanish]  = "Objetivos defendidos" } }, 
            },
        };
    }
}