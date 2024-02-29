using System.Collections.Generic;
using System.Threading.Tasks;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class MiscStatCreator
    {
        public static async Task<List<Stat>> CreateMiscStats()
        {
            var miscStats = CreateCustomMiscStats();
            miscStats.AddRange(await ItemStatsCreator.CreateMiscItemStats());
            return miscStats;
        }

        public static List<Stat> CreateCustomMiscStats()
        {
            return new List<Stat>()
            {
                new Stat
                {
                    Id   = StatId.LUCK,
                    Icon = { FileName = "luck.png" },
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Luck",
                            [Locale.German]  = "Glück",
                            [Locale.French]  = "Chance",
                            [Locale.Spanish] = "Suerte"
                        }
                    }
                },
                new Stat
                {
                    Id   = StatId.DEATHS,
                    Icon = { FileName = "death.png" },
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Deaths",
                            [Locale.German]  = "Tode",
                            [Locale.French]  = "Morts",
                            [Locale.Spanish] = "Muertes"
                        }
                    },
                    Description =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Combined deaths from all sources (WvW, sPvP, PvE)",
                            [Locale.German]  = "Aufsummierte Tode aus allen Quellen (WvW, sPvP, PvE)",
                            [Locale.French]  = "Nombre total de morts venant de toutes les sources (JcE, JcJ, McM)",
                            [Locale.Spanish] = "Número total de muertes de todas las fuentes (McM, JcJ, JcE)"
                        }
                    },
                },
            };
        }
    }
}