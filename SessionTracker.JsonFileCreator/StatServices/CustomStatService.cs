using System.Collections.Generic;
using Gw2Sharp.WebApi;
using SessionTracker.Models;
using SessionTracker.Models.Constants;

namespace SessionTracker.JsonFileCreator.StatServices
{
    public class CustomStatService
    {
        public static readonly List<Entry> CustomStats = new List<Entry>
        {
            new Entry { Id = EntryId.LUCK, Name = { LocalizedTextByLocale = { [Locale.English] = "Luck", [Locale.German] = "Glück", [Locale.French] = "Chance", [Locale.Spanish] = "Suerte" } }, IconFileName = "luck.png" },
            new Entry
            {
                Id           = EntryId.DEATHS,
                IconFileName = "death.png",
                Name         = { LocalizedTextByLocale = { [Locale.English] = "Deaths", [Locale.German]                                            = "Tode", [Locale.French]                                                 = "Morts", [Locale.Spanish]                                                              = "Muertes" } },
                Description  = { LocalizedTextByLocale = { [Locale.English] = "Combined deaths from all sources (WvW, sPvP, PvE)", [Locale.German] = "Aufsummierte Tode aus allen Quellen (WvW, sPvP, PvE)", [Locale.French] = "Nombre total de morts venant de toutes les sources (JcE, JcJ, McM)", [Locale.Spanish] = "Número total de muertes de todas las fuentes (McM, JcJ, JcE)" } },
            },
            new Entry { Id = EntryId.PVP_KILLS, Name = { LocalizedTextByLocale = { [Locale.English] = "PvP kills", [Locale.German] = "PvP Feinde besiegt", [Locale.French] = "Victimes JcJ", [Locale.Spanish] = "PvP bajas" } }, IconFileName = "kill.png", ApiId = 239, ApiIdType = ApiIdType.Achievement },
            new Entry
            {
                Id           = EntryId.PVP_KDR,
                IconFileName = "kdr.png",
                Name         = { LocalizedTextByLocale = { [Locale.English] = "PvP KDR", [Locale.German]                = "PvP KDR", [Locale.French]                             = "KDR JcJ", [Locale.Spanish]                  = "PvP B/M" } },
                Description  = { LocalizedTextByLocale = { [Locale.English] = "PvP kills/deaths ratio", [Locale.German] = "PvP Verhältnis besiegte Feinde/Tode", [Locale.French] = "Ratio Victimes/Morts JcJ", [Locale.Spanish] = "PvP Ratio de Bajas/Muertes" } }
            },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, Name      = { LocalizedTextByLocale = { [Locale.English] = "PvP total wins", [Locale.German]      = "PvP gesamt gewonnen", [Locale.French]           = "Victoires JcJ", [Locale.Spanish]              = "PvP Victorias totales" } }, IconFileName            = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, Name    = { LocalizedTextByLocale = { [Locale.English] = "PvP total losses", [Locale.German]    = "PvP gesamt verloren", [Locale.French]           = "Défaites JcJ", [Locale.Spanish]               = "PvP Derrotas totales" } }, IconFileName             = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, Name     = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked wins", [Locale.German]     = "PvP mit rangwertung gewonnen", [Locale.French]  = "Victoires JcJ classées", [Locale.Spanish]     = "PvP Victorias en clasificatorias" } }, IconFileName = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked losses", [Locale.German]   = "PvP mit rangwertung verloren", [Locale.French]  = "Défaites JcJ classées", [Locale.Spanish]      = "PvP Derrotas en clasificatorias" } }, IconFileName  = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked wins", [Locale.German]   = "PvP ohne rangwertung gewonnen", [Locale.French] = "Victoires JcJ non classées", [Locale.Spanish] = "PvP Victorias en libres" } }, IconFileName          = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, Name = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked losses", [Locale.German] = "PvP ohne rangwertung verloren", [Locale.French] = "Défaites JcJ non classées", [Locale.Spanish]  = "PvP Derrotas en libres" } }, IconFileName           = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, Name     = { LocalizedTextByLocale = { [Locale.English] = "PvP custom wins", [Locale.German]     = "Pvp selbsterstellt gewonnen", [Locale.French]   = "Victoires JcJ personnalisé", [Locale.Spanish] = "PvP Victorias en personalidadas" } }, IconFileName  = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP custom losses", [Locale.German]   = "PvP selbsterstellt verloren", [Locale.French]   = "Défaites JcJ personnalisé", [Locale.Spanish]  = "PvP Derrotas en personalidadas" } }, IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, Name            = { LocalizedTextByLocale = { [Locale.English] = "PvP rank", [Locale.German]            = "PvP rang", [Locale.French]                      = "Rang JcJ", [Locale.Spanish]                   = "PvP Rango" } }, IconFileName                        = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, Name  = { LocalizedTextByLocale = { [Locale.English] = "PvP ranking points", [Locale.German]  = "PvP rang punkte", [Locale.French]               = "Points de classement JcJ", [Locale.Spanish]   = "PvP Puntos de clasificación" } }, IconFileName      = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, Name           = { LocalizedTextByLocale = { [Locale.English] = "WvW kills", [Locale.German]           = "WvW Feinde besiegt", [Locale.French]            = "Victimes McM", [Locale.Spanish]               = "McM Bajas" } }, IconFileName                        = "kill.png", ApiId = 283, ApiIdType = ApiIdType.Achievement },
            new Entry
            {
                Id           = EntryId.WVW_KDR,
                IconFileName = "kdr.png",
                Name         = { LocalizedTextByLocale = { [Locale.English] = "WvW KDR", [Locale.German]                = "WvW KDR", [Locale.French]                             = "KDR McM", [Locale.Spanish]                     = "McM B/M" } },
                Description  = { LocalizedTextByLocale = { [Locale.English] = "WvW kills/deaths ratio", [Locale.German] = "WvW Verhältnis besiegte Feinde/Tode", [Locale.French] = "Ratio Victimes/Morts en McM", [Locale.Spanish] = "McM Ratio de Bajas/Muertes" } }
            },
            new Entry { Id = EntryId.WVW_RANK, Name = { LocalizedTextByLocale = { [Locale.English] = "WvW rank", [Locale.German] = "WvW rang", [Locale.French] = "Rang McM", [Locale.Spanish] = "McM Rango" } }, IconFileName = "wvwRank.png" },
            new Entry
            {
                Id          = EntryId.WVW_SUPPLY_REPAIR,
                ApiId       = 306,
                ApiIdType   = ApiIdType.Achievement,
                Name        = { LocalizedTextByLocale = { [Locale.English] = "Supply (repair)", [Locale.German]         = "Vorräte (reparieren)", [Locale.French]                 = "Ravitaillement (réparations)", [Locale.Spanish]                            = "Suministros (reparaciones)" } }, IconFileName = "supplySpend.png",
                Description = { LocalizedTextByLocale = { [Locale.English] = "Supply spent on repairs", [Locale.German] = "Durch Reparieren verbrauchte Vorräte", [Locale.French] = "Quantité de ravitaillement utilisé pour les réparations", [Locale.Spanish] = "Suministros gastados en reparaciones" } }
            },
            new Entry { Id = "wvw dolyaks killed", Name      = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks killed", [Locale.German]      = "Karawane besiegt", [Locale.French]    = "Caravanes tuées", [Locale.Spanish]     = "Caravanas asesinadas" } }, IconFileName   = "dolyak.png", ApiId            = 288, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw dolyaks escorted", Name    = { LocalizedTextByLocale = { [Locale.English] = "Dolyaks escorted", [Locale.German]    = "Karawane verteidgt", [Locale.French]  = "Caravanes escortées", [Locale.Spanish] = "Caravanas escoltadas" } }, IconFileName   = "dolyakDefended.png", ApiId    = 285, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw camps captured", Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps captured", [Locale.German]      = "Lager erobert", [Locale.French]       = "Camps capturés", [Locale.Spanish]      = "Campamentos capturados" } }, IconFileName = "camp.png", ApiId              = 291, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw camps defended", Name      = { LocalizedTextByLocale = { [Locale.English] = "Camps defended", [Locale.German]      = "Lager verteidigt", [Locale.French]    = "Camps défendus", [Locale.Spanish]      = "Campamentos defendidos" } }, IconFileName = "campDefended.png", ApiId      = 310, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw towers captured", Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers captured", [Locale.German]     = "Türme erobert", [Locale.French]       = "Tours capturées", [Locale.Spanish]     = "Torres capturadas" } }, IconFileName      = "tower.png", ApiId             = 297, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw towers defended", Name     = { LocalizedTextByLocale = { [Locale.English] = "Towers defended", [Locale.German]     = "Türme verteidigt", [Locale.French]    = "Tours défendues", [Locale.Spanish]     = "Torres defendidas" } }, IconFileName      = "towerDefended.png", ApiId     = 322, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw keeps captured", Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps captured", [Locale.German]      = "Festungen erobert", [Locale.French]   = "Forts capturés", [Locale.Spanish]      = "Fortalezas capturadas" } }, IconFileName  = "keep.png", ApiId              = 300, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw keeps defended", Name      = { LocalizedTextByLocale = { [Locale.English] = "Keeps defended", [Locale.German]      = "Festungen verteidgt", [Locale.French] = "Forts défendus", [Locale.Spanish]      = "Fortalezas defendidas" } }, IconFileName  = "keepDefended.png", ApiId      = 316, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw castles captured", Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles captured", [Locale.German]    = "Schloss erobert", [Locale.French]     = "Châteaux capturés", [Locale.Spanish]   = "Castillos capturados" } }, IconFileName   = "castle.png", ApiId            = 294, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw castles defended", Name    = { LocalizedTextByLocale = { [Locale.English] = "Castles defended", [Locale.German]    = "Schloss verteidgt", [Locale.French]   = "Châteaux défendus", [Locale.Spanish]   = "Castillos defendidos" } }, IconFileName   = "castleDefended.png", ApiId    = 313, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw objectives captured", Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives captured", [Locale.German] = "Objekte erobert", [Locale.French]     = "Objectifs capturés", [Locale.Spanish]  = "Objetivos capturados" } }, IconFileName   = "objective.png", ApiId         = 303, ApiIdType = ApiIdType.Achievement },
            new Entry { Id = "wvw objectives defended", Name = { LocalizedTextByLocale = { [Locale.English] = "Objectives defended", [Locale.German] = "Objekte verteidigt", [Locale.French]  = "Objectifs défendus", [Locale.Spanish]  = "Objetivos defendidos" } }, IconFileName   = "objectiveDefended.png", ApiId = 319, ApiIdType = ApiIdType.Achievement },
        };
    }
}