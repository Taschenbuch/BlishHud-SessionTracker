using System.IO;
using System.Threading.Tasks;
using SessionTracker.Files;
using SessionTracker.Models;
using System;
using System.Collections.Generic;
using SessionTracker.JsonFileCreator.StatCreators;

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
            model.Stats.AddRange(MiscStatCreator.CreateMiscStats());
            model.Stats.AddRange(WvwStatsCreator.CreateWvwStats());
            model.Stats.AddRange(PvpStatsCreator.CreatePvpStats());
            model.Stats.AddRange(await CurrencyStatsCreator.CreateCurrencyStats());
            model.Stats.AddRange(await ItemStatsCreator.CreateItemStats());
            ThrowIfStatOrderIsZero(model.Stats);
        }

        private static void ThrowIfStatOrderIsZero(List<Stat> stats)
        {
            foreach (Stat stat in stats)
                if(stat.Position == 0)
                    throw new Exception($"Error: Order must be >0. stat: {stat.Name.English} (id: {stat.Id}, apiId: {stat.ApiId})");
        }
    }
}