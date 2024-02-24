using System.IO;
using System.Threading.Tasks;
using SessionTracker.Files;
using SessionTracker.Models;
using System;
using SessionTracker.JsonFileCreator.StatCreators;
using System.Collections.Generic;
using SessionTracker.JsonFileCreator.OtherCreators;

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
            var stats = await CreateStats(categories);
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

        private static async Task<List<Stat>> CreateStats(List<StatCategory> categories)
        {
            var stats = new List<Stat>();
            stats.AddRange(WvwStatsCreator.CreateWvwStats());
            stats.AddRange(PvpStatsCreator.CreatePvpStats());
            stats.AddRange(await MiscStatCreator.CreateMiscStats());
            stats.AddRange(await CurrencyStatsCreator.CreateCurrencyStats());
            stats.AddRange(await ItemStatsCreator.CreateMaterialStorageItemStats(categories));
            return stats;
        }

        private static void WriteModelToFile(Model model)
        {
            var jsonModel = JsonService.SerializeModelToJson(model);
            File.WriteAllText(MODEL_FILE_PATH, jsonModel);
        }
    }
}