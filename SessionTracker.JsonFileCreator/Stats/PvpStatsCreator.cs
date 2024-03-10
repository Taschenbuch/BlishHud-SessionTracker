using System.Collections.Generic;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.Stats
{
    public class PvpStatsCreator
    {
        public static List<Stat> CreatePvpStats()
        {
            return new List<Stat>()
            {
                new Stat
                {
                    Id           = StatId.PVP_KILLS,
                    Icon         = { FileName = "kill.png" },
                    ApiId        = 239,
                    ApiIdType    = ApiIdType.Achievement,
                    Name = { LocalizedTextByLocale = { [Locale.English] = "PvP kills", [Locale.German] = "PvP Feinde besiegt", [Locale.French] = "Victimes JcJ", [Locale.Spanish] = "PvP bajas" } }
                },
                new Stat
                {
                    Id           = StatId.PVP_KDR,
                    Icon         = { FileName = "kdr.png" },
                    Name         = { LocalizedTextByLocale = { [Locale.English] = "PvP KDR", [Locale.German]                = "PvP KDR", [Locale.French]                             = "KDR JcJ", [Locale.Spanish]                  = "PvP B/M" } },
                    Description  = { LocalizedTextByLocale = { [Locale.English] = "PvP kills/deaths ratio", [Locale.German] = "PvP Verhältnis besiegte Feinde/Tode", [Locale.French] = "Ratio Victimes/Morts JcJ", [Locale.Spanish] = "PvP Ratio de Bajas/Muertes" } }
                },
                new Stat { Id = StatId.PVP_RANK, Name            = { LocalizedTextByLocale = { [Locale.English] = "PvP rank", [Locale.German]            = "PvP Rang", [Locale.French]                      = "Rang JcJ", [Locale.Spanish]                   = "PvP Rango" } }, Icon = { FileName = "pvpRank.png" } },
                new Stat { Id = StatId.PVP_RANKING_POINTS, Name  = { LocalizedTextByLocale = { [Locale.English] = "PvP ranking points", [Locale.German]  = "PvP Rangpunkte", [Locale.French]                = "Points de classement JcJ", [Locale.Spanish]   = "PvP Puntos de clasificación" } }, Icon = { FileName = "pvpRankingPoints.png" } },
                new Stat { Id = StatId.PVP_TOTAL_WINS,  Name     = { LocalizedTextByLocale = { [Locale.English] = "PvP total wins", [Locale.German]      = "PvP gesamt gewonnen", [Locale.French]           = "Victoires JcJ", [Locale.Spanish]              = "PvP Victorias totales" } }, Icon = { FileName = "pvpWins.png" } },
                new Stat { Id = StatId.PVP_TOTAL_LOSSES, Name    = { LocalizedTextByLocale = { [Locale.English] = "PvP total losses", [Locale.German]    = "PvP gesamt verloren", [Locale.French]           = "Défaites JcJ", [Locale.Spanish]               = "PvP Derrotas totales" } }, Icon = { FileName = "pvpLosses.png" } },
                new Stat { Id = StatId.PVP_RANKED_WINS, Name     = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked wins", [Locale.German]     = "PvP mit Rangwertung gewonnen", [Locale.French]  = "Victoires JcJ classées", [Locale.Spanish]     = "PvP Victorias en clasificatorias" } }, Icon = { FileName = "pvpWins.png" } },
                new Stat { Id = StatId.PVP_RANKED_LOSSES, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP ranked losses", [Locale.German]   = "PvP mit Rangwertung verloren", [Locale.French]  = "Défaites JcJ classées", [Locale.Spanish]      = "PvP Derrotas en clasificatorias" } }, Icon = { FileName = "pvpLosses.png" } },
                new Stat { Id = StatId.PVP_UNRANKED_WINS, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked wins", [Locale.German]   = "PvP ohne Rangwertung gewonnen", [Locale.French] = "Victoires JcJ non classées", [Locale.Spanish] = "PvP Victorias en libres" } }, Icon = { FileName = "pvpWins.png" } },
                new Stat { Id = StatId.PVP_UNRANKED_LOSSES, Name = { LocalizedTextByLocale = { [Locale.English] = "PvP unranked losses", [Locale.German] = "PvP ohne Rangwertung verloren", [Locale.French] = "Défaites JcJ non classées", [Locale.Spanish]  = "PvP Derrotas en libres" } }, Icon = { FileName = "pvpLosses.png" } },
                new Stat { Id = StatId.PVP_CUSTOM_WINS, Name     = { LocalizedTextByLocale = { [Locale.English] = "PvP custom wins", [Locale.German]     = "Pvp selbsterstellt gewonnen", [Locale.French]   = "Victoires JcJ personnalisé", [Locale.Spanish] = "PvP Victorias en personalidadas" } }, Icon = { FileName = "pvpWins.png" } },
                new Stat { Id = StatId.PVP_CUSTOM_LOSSES, Name   = { LocalizedTextByLocale = { [Locale.English] = "PvP custom losses", [Locale.German]   = "PvP selbsterstellt verloren", [Locale.French]   = "Défaites JcJ personnalisé", [Locale.Spanish]  = "PvP Derrotas en personalidadas" } }, Icon = { FileName = "pvpLosses.png" } },
            };
        }
    }
}