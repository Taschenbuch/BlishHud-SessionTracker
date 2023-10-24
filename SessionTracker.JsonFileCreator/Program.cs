using System.IO;
using System.Threading.Tasks;
using SessionTracker.Files;
using SessionTracker.JsonFileCreator.StatServices;
using SessionTracker.Models;
using System;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        private const string MODEL_FILE_PATH = @"C:\Dev\blish\model.json";

        static async Task Main()
        {
            Console.WriteLine("create model.json...");
            var model = new Model();
            await AddStatsToModel(model);
            var jsonModel = JsonService.SerializeModelToJson(model);
            File.WriteAllText(MODEL_FILE_PATH, jsonModel);
            Console.WriteLine($"create model.json finished: {MODEL_FILE_PATH}");
        }

        private static async Task AddStatsToModel(Model model)
        {
            var itemStats     = await ItemStatService.CreateItemStats();
            var currencyStats = await CurrencyStatService.CreateCurrencyStats();

            model.Stats.AddRange(itemStats);
            model.Stats.AddRange(CustomStatService.CustomStats);
            model.Stats.AddRange(currencyStats);
        }
    }
}