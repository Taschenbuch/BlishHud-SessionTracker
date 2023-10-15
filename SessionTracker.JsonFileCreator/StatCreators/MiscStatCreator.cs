using System.Collections.Generic;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class MiscStatCreator
    {
        public static List<Stat> CreateMiscStats()
        {
            var miscStats = SetCategory(_miscStats);
            miscStats = StatsCreatorCommon.SetOrder(miscStats);
            return miscStats;
        }

        private static List<Stat> SetCategory(List<Stat> stats)
        {
            foreach (var stat in stats)
            {
                stat.Category.Name.LocalizedTextByLocale = new Dictionary<Locale, string>()
                {
                    { Locale.English, "Misc" },
                    { Locale.French, "Autre" },
                    { Locale.German, "Sonstiges" },
                    { Locale.Spanish, "Otros" },
                };
                stat.Category.Type = StatCategoryType.Misc;
            }

            return stats;
        }

        private static readonly List<Stat> _miscStats = new List<Stat>
        {
            new Stat 
            { 
                Id = StatId.LUCK,
                IconFileName = "luck.png",
                Name = { LocalizedTextByLocale = { [Locale.English] = "Luck", [Locale.German] = "Glück", [Locale.French] = "Chance", [Locale.Spanish] = "Suerte" } }
            },
            new Stat
            {
                Id           = StatId.DEATHS,
                IconFileName = "death.png",
                Name         = { LocalizedTextByLocale = { [Locale.English] = "Deaths", [Locale.German]                                            = "Tode", [Locale.French]                                                 = "Morts", [Locale.Spanish]                                                              = "Muertes" } },
                Description  = { LocalizedTextByLocale = { [Locale.English] = "Combined deaths from all sources (WvW, sPvP, PvE)", [Locale.German] = "Aufsummierte Tode aus allen Quellen (WvW, sPvP, PvE)", [Locale.French] = "Nombre total de morts venant de toutes les sources (JcE, JcJ, McM)", [Locale.Spanish] = "Número total de muertes de todas las fuentes (McM, JcJ, JcE)" } },
            },
        };
    }
}