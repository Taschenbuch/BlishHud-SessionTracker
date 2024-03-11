using System.IO;
using System.Threading.Tasks;
using SessionTracker.Files;
using SessionTracker.Models;
using System;
using SessionTracker.JsonFileCreator.Stats;
using System.Collections.Generic;
using SessionTracker.JsonFileCreator.Other;
using SessionTracker.JsonFileCreator.Category;
using SessionTracker.JsonFileCreator.Constants;
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

            CreatorCommon.AddStatIdsToCategory(CategoryId.WVW, categories, CategoryStatIdCollector.GetWvwStatIds(wvwStats));
            CreatorCommon.AddStatIdsToCategory(CategoryId.PVP, categories, CategoryStatIdCollector.GetPvpStatIds(pvpStats));
            CreatorCommon.AddStatIdsToCategory(CategoryId.MISC, categories, CategoryStatIdCollector.GetMiscStatIds());
            CreatorCommon.AddStatIdsToCategory(CategoryId.CURRENCY, categories, CategoryStatIdCollector.GetCurrencyStatIds(currencyStats));
            CreatorCommon.AddStatIdsToCategory(CategoryId.FESTIVAL, categories, CategoryStatIdCollector.GetFestivalStatIds());
            CreatorCommon.AddStatIdsToCategory(CategoryId.FRACTAL, categories, CategoryStatIdCollector.GetFractalStatIds());
            CreatorCommon.AddStatIdsToCategory(CategoryId.RAID, categories, CategoryStatIdCollector.GetRaidStatIds());
            CreatorCommon.AddStatIdsToCategory(CategoryId.STRIKE, categories, CategoryStatIdCollector.GetStrikeStatIds());
            CreatorCommon.AddStatIdsToCategory(CategoryId.OPEN_WORLD, categories, CategoryStatIdCollector.GetOpenWorldStatIds());

            var stats = new List<Stat>();
            stats.AddRange(wvwStats);
            stats.AddRange(pvpStats);
            stats.AddRange(miscStats);
            stats.AddRange(currencyStats);
            stats.AddRange(materialStorageStats);
            stats = RemoveStatDuplicates(stats);
            return stats;
        }

        // duplicate examples: Mystic Coin (raid, fractal, material storage), Memory of Battle (wvw, material storage)
        private static List<Stat> RemoveStatDuplicates(List<Stat> stats)
        {
            return stats
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();
        }

        private static void WriteModelToFile(Model model)
        {
            var jsonModel = JsonService.SerializeModelToJson(model);
            File.WriteAllText(MODEL_FILE_PATH, jsonModel);
        }
    }
}