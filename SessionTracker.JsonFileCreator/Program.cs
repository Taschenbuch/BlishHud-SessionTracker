using System.IO;
using System.Threading.Tasks;
using SessionTracker.Files;
using SessionTracker.Models;
using System;
using SessionTracker.JsonFileCreator.StatCreators;
using System.Collections.Generic;
using SessionTracker.JsonFileCreator.OtherCreators;
using SessionTracker.Constants;
using System.Linq;

namespace SessionTracker.JsonFileCreator
{
    public class Program
    {
        private const string MODEL_FILE_PATH = @"C:\Dev\blish\model.json";

        static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("create model.json...");

            var categories = await CategoryCreator.CreateCategories();
            var stats = await CreateStatsAndAddTheirIdsToCategories(categories);
            var model = CreateModel(categories, stats);
            ModelValidatorService.ThrowIfModelIsInvalid(model);
            WriteModelToFile(model);
            
            Console.WriteLine($"create model.json finished: {MODEL_FILE_PATH}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static Model CreateModel(List<StatCategory> categories, List<Stat> stats)
        {
            var model = new Model();
            model.Stats.AddRange(stats);
            model.StatCategories.AddRange(categories);
            return model;
        }

        private static async Task<List<Stat>> CreateStatsAndAddTheirIdsToCategories(List<StatCategory> categories)
        {
            var wvwStats = WvwStatsCreator.CreateWvwStats();
            var pvpStats = PvpStatsCreator.CreatePvpStats();
            var miscStats = await MiscStatCreator.CreateMiscStats();
            var currencyStats = await CurrencyStatsCreator.CreateCurrencyStats();
            var materialStorageStats = await ItemStatsCreator.CreateMaterialStorageItemStats(categories);

            // todo x: method/class die statIds für Categories erzeugen?
            var wvwStatIds = new List<string>() { StatId.DEATHS };
            wvwStatIds.AddRange(wvwStats.Select(s => s.Id));
            wvwStatIds.AddRange(CurrencyIds.Wvw.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            wvwStatIds.AddRange(ItemIds.Wvw.Select(i => CreatorCommon.CreateItemStatId(i)));

            var pvpStatIds = new List<string>() { StatId.DEATHS };
            pvpStatIds.AddRange(pvpStats.Select(s => s.Id));
            pvpStatIds.AddRange(CurrencyIds.Pvp.Select(i => CreatorCommon.CreateCurrencyStatId(i)));

            var miscStatIds = new List<string>()
            {
                StatId.DEATHS,
                StatId.LUCK,
                CreatorCommon.CreateItemStatId(ItemIds.TRICK_OR_TREAT_BAG),
            };

            var currencyStatIds = currencyStats.Select(i => i.Id).ToList();

            var fractalStatIds = new List<string>();
            fractalStatIds.AddRange(CurrencyIds.Fractal.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            fractalStatIds.AddRange(ItemIds.Fractal.Select(i => CreatorCommon.CreateItemStatId(i)));

            var raidStatIds = new List<string>();
            raidStatIds.AddRange(CurrencyIds.Raid.Select(i => CreatorCommon.CreateCurrencyStatId(i)));
            raidStatIds.AddRange(ItemIds.Raid.Select(i => CreatorCommon.CreateItemStatId(i)));

            var strikeStatIds = new List<string>();
            strikeStatIds.AddRange(CurrencyIds.Strike.Select(i => CreatorCommon.CreateCurrencyStatId(i)));

            var openWorldStatIds = new List<string>();
            openWorldStatIds.AddRange(CurrencyIds.OpenWorld.Select(i => CreatorCommon.CreateCurrencyStatId(i)));

            CreatorCommon.AddStatIdsToCategory(CategoryId.WVW, categories, wvwStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.PVP, categories, pvpStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.MISC, categories, miscStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.CURRENCY, categories, currencyStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.FRACTAL, categories, fractalStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.RAID, categories, raidStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.STRIKE, categories, strikeStatIds);
            CreatorCommon.AddStatIdsToCategory(CategoryId.OPEN_WORLD, categories, openWorldStatIds);

            var stats = new List<Stat>();
            stats.AddRange(wvwStats);
            stats.AddRange(pvpStats);
            stats.AddRange(miscStats);
            stats.AddRange(currencyStats);
            stats.AddRange(materialStorageStats);
            return stats;
        }

        private static void WriteModelToFile(Model model)
        {
            var jsonModel = JsonService.SerializeModelToJson(model);
            File.WriteAllText(MODEL_FILE_PATH, jsonModel);
        }
    }
}