using SessionTracker.Constants;
using SessionTracker.JsonFileCreator.Constants;
using SessionTracker.JsonFileCreator.Other;
using SessionTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.JsonFileCreator.Category
{
    public class CategoryStatIdCollector
    {
        public static List<string> GetOpenWorldStatIds()
        {
            var openWorldStatIds = new List<string>();
            openWorldStatIds.AddRange(CurrencyIds.OpenWorld.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            return openWorldStatIds;
        }

        public static List<string> GetStrikeStatIds()
        {
            var strikeStatIds = new List<string>();
            strikeStatIds.AddRange(CurrencyIds.Strike.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            return strikeStatIds;
        }

        public static List<string> GetRaidStatIds()
        {
            var raidStatIds = new List<string>();
            raidStatIds.AddRange(CurrencyIds.Raid.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            raidStatIds.AddRange(ItemIds.Raid.Select(i => CreatorCommon.CreateItemStatId(i)));
            return raidStatIds;
        }

        public static List<string> GetFractalStatIds()
        {
            var fractalStatIds = new List<string>();
            fractalStatIds.AddRange(CurrencyIds.Fractal.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            fractalStatIds.AddRange(ItemIds.Fractal.Select(i => CreatorCommon.CreateItemStatId(i)));
            return fractalStatIds;
        }

        public static List<string> GetCurrencyStatIds(List<Stat> currencyStats)
        {
            return currencyStats.Select(i => i.Id).ToList();
        }

        public static List<string> GetMiscStatIds()
        {
            var miscStatIds = new List<string>()
            {
                StatId.DEATHS,
                StatId.LUCK,
            };
            miscStatIds.AddRange(ItemIds.Misc.Select(i => CreatorCommon.CreateItemStatId(i)));
            return miscStatIds;
        }

        public static List<string> GetPvpStatIds(List<Stat> pvpStats)
        {
            var pvpStatIds = new List<string>()
            {
                StatId.DEATHS
            };
            pvpStatIds.AddRange(pvpStats.Select(s => s.Id));
            pvpStatIds.AddRange(CurrencyIds.Pvp.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            return pvpStatIds;
        }

        public static List<string> GetWvwStatIds(List<Stat> wvwStats)
        {
            var wvwStatIds = new List<string>()
            {
                StatId.DEATHS
            };
            wvwStatIds.AddRange(wvwStats.Select(s => s.Id));
            wvwStatIds.AddRange(CurrencyIds.Wvw.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            wvwStatIds.AddRange(ItemIds.Wvw.Select(i => CreatorCommon.CreateItemStatId(i)));
            return wvwStatIds;
        }

        // material storage has its own festival sub category, but for now it wont be included here. too much duplication.
        // Though I may change my mind about this in the future.
        public static List<string> GetFestivalStatIds()
        {
            var festivalStatIds = new List<string>();
            festivalStatIds.AddRange(ItemIds.Festival.Select(i => CreatorCommon.CreateItemStatId(i)));
            return festivalStatIds;
        }
    }
}
