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
            ModelValidator.ThrowIfModelIsInvalid(model);
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

            var wvwStatIds = CategoryStatIdCollector.GetWvwStatIds(wvwStats);
            var pvpStatIds = CategoryStatIdCollector.GetPvpStatIds(pvpStats);
            var miscStatIds = CategoryStatIdCollector.GetMiscStatIds();
            var currencyStatIds = CategoryStatIdCollector.GetCurrencyStatIds(currencyStats);
            var fractalStatIds = CategoryStatIdCollector.GetFractalStatIds();
            var raidStatIds = CategoryStatIdCollector.GetRaidStatIds();
            var strikeStatIds = CategoryStatIdCollector.GetStrikeStatIds();
            var openWorldStatIds = CategoryStatIdCollector.GetOpenWorldStatIds();

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