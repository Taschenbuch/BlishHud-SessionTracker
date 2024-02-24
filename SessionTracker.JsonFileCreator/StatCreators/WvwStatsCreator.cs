using System.Collections.Generic;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.JsonFileCreator.OtherCreators;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class WvwStatsCreator
    {
        public static List<Stat> CreateWvwStats()
        {
            var wvwStats = CreateStats();
            CreatorCommon.SetPositionInCategoryAndCategoryId(wvwStats, CategoryId.WVW);
            return wvwStats;
        }

        private static List<Stat> CreateStats()
        {
            return new List<Stat>()
            {
                new Stat
                {
                    Id = StatId.WVW_KILLS,
                    Icon = { FileName = "kill.png" },
                    ApiId = 283,
                    ApiIdType = ApiIdType.Achievement,
                    Name = { LocalizedTextByLocale = { [Locale.English] = "WvW kills", [Locale.German]           = "WvW Feinde besiegt", [Locale.French]            = "Victimes McM", [Locale.Spanish]               = "McM Bajas" } },
                },
                new Stat
                {
                    Id           = StatId.WVW_KDR,
                    Icon         = { FileName = "kdr.png" },
                    Name         = { LocalizedTextByLocale = { [Locale.English] = "WvW KDR", [Locale.German]                = "WvW KDR", [Locale.French]                             = "KDR McM", [Locale.Spanish]                     = "McM B/M" } },
                    Description  = { LocalizedTextByLocale = { [Locale.English] = "WvW kills/deaths ratio", [Locale.German] = "WvW Verhältnis besiegte Feinde/Tode", [Locale.French] = "Ratio Victimes/Morts en McM", [Locale.Spanish] = "McM Ratio de Bajas/Muertes" } }
                },
                new Stat
                {
                    Id = StatId.WVW_RANK,
                    Icon = { FileName = "wvwRank.png" },
                    Name = { LocalizedTextByLocale = { [Locale.English] = "WvW rank", [Locale.German] = "WvW rang", [Locale.French] = "Rang McM", [Locale.Spanish] = "McM Rango" } },
                },
                new Stat
                {
                    Id          = StatId.WVW_SUPPLY_REPAIR,
                    Icon        = { FileName = "supplySpend.png" },
                    ApiId       = 306,
                    ApiIdType   = ApiIdType.Achievement,
                    Name        = { LocalizedTextByLocale = { [Locale.English] = "Supply (repair)", [Locale.German]         = "Vorräte (reparieren)", [Locale.French]                 = "Ravitaillement (réparations)", [Locale.Spanish]                            = "Suministros (reparaciones)" } }, 
                    Description = { LocalizedTextByLocale = { [Locale.English] = "Supply spent on repairs", [Locale.German] = "Durch Reparieren verbrauchte Vorräte", [Locale.French] = "Quantité de ravitaillement utilisé pour les réparations", [Locale.Spanish] = "Suministros gastados en reparaciones" } }
                },
                new Stat
                {
                    Id   = "wvw dolyaks killed",
                    Icon = { FileName = "dolyak.png" },
                    ApiId            = 288,
                    ApiIdType = ApiIdType.Achievement,
                    Name = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks killed", [Locale.German]      = "Karawane besiegt", [Locale.French]    = "Caravanes tuées", [Locale.Spanish]     = "Caravanas asesinadas" } },
                },
                new Stat
                {
                    Id = "wvw dolyaks escorted",
                    Icon = { FileName = "dolyakDefended.png" },
                    ApiId    = 285,
                    ApiIdType = ApiIdType.Achievement,
                    Name    = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks escorted", [Locale.German]    = "Karawane verteidgt", [Locale.French]  = "Caravanes escortées", [Locale.Spanish] = "Caravanas escoltadas" } },
                },
                new Stat
                {
                    Id = "wvw camps captured",
                    Icon = { FileName = "camp.png" },
                    ApiId              = 291,
                    ApiIdType = ApiIdType.Achievement,
                    Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps captured", [Locale.German]      = "Lager erobert", [Locale.French]       = "Camps capturés", [Locale.Spanish]      = "Campamentos capturados" } },
                },
                new Stat
                {
                    Id = "wvw camps defended",
                    Icon = { FileName = "campDefended.png" },
                    ApiId      = 310,
                    ApiIdType = ApiIdType.Achievement,
                    Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps defended", [Locale.German]      = "Lager verteidigt", [Locale.French]    = "Camps défendus", [Locale.Spanish]      = "Campamentos defendidos" } },
                },
                new Stat
                {
                    Id = "wvw towers captured",
                    Icon = { FileName = "tower.png" },
                    ApiId             = 297,
                    ApiIdType = ApiIdType.Achievement,
                    Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers captured", [Locale.German]     = "Türme erobert", [Locale.French]       = "Tours capturées", [Locale.Spanish]     = "Torres capturadas" } },
                },
                new Stat
                {
                    Id = "wvw towers defended",
                    Icon = { FileName = "towerDefended.png" },
                    ApiId     = 322,
                    ApiIdType = ApiIdType.Achievement,
                    Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers defended", [Locale.German]     = "Türme verteidigt", [Locale.French]    = "Tours défendues", [Locale.Spanish]     = "Torres defendidas" } },
                },
                new Stat
                {
                    Id = "wvw keeps captured",
                    Icon = { FileName = "keep.png" },
                    ApiId              = 300,
                    ApiIdType = ApiIdType.Achievement,
                    Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps captured", [Locale.German]      = "Festungen erobert", [Locale.French]   = "Forts capturés", [Locale.Spanish]      = "Fortalezas capturadas" } },
                },
                new Stat
                {
                    Id = "wvw keeps defended",
                    Icon = { FileName = "keepDefended.png" },
                    ApiId      = 316,
                    ApiIdType = ApiIdType.Achievement,
                    Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps defended", [Locale.German]      = "Festungen verteidgt", [Locale.French] = "Forts défendus", [Locale.Spanish]      = "Fortalezas defendidas" } },
                },
                new Stat
                {
                    Id = "wvw castles captured",
                    Icon = { FileName = "castle.png" },
                    ApiId            = 294,
                    ApiIdType = ApiIdType.Achievement,
                    Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles captured", [Locale.German]    = "Schloss erobert", [Locale.French]     = "Châteaux capturés", [Locale.Spanish]   = "Castillos capturados" } },
                },
                new Stat
                {
                    Id = "wvw castles defended",
                    Icon = { FileName = "castleDefended.png" },
                    ApiId    = 313,
                    ApiIdType = ApiIdType.Achievement,
                    Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles defended", [Locale.German]    = "Schloss verteidgt", [Locale.French]   = "Châteaux défendus", [Locale.Spanish]   = "Castillos defendidos" } },
                },
                new Stat
                {
                    Id = "wvw objectives captured",
                    Icon = { FileName = "objective.png" },
                    ApiId         = 303,
                    ApiIdType = ApiIdType.Achievement,
                    Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives captured", [Locale.German] = "Objekte erobert", [Locale.French]     = "Objectifs capturés", [Locale.Spanish]  = "Objetivos capturados" } },
                },
                new Stat
                {
                    Id = "wvw objectives defended",
                    Icon = { FileName = "objectiveDefended.png" },
                    ApiId = 319,
                    ApiIdType = ApiIdType.Achievement,
                    Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives defended", [Locale.German] = "Objekte verteidigt", [Locale.French]  = "Objectifs défendus", [Locale.Spanish]  = "Objetivos defendidos" } },
                },
            };
        }
    }
}