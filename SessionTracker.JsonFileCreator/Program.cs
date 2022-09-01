using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SessionTracker.Models;
using SessionTracker.Settings;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        static async Task Main()
        {
            var model = new Model();
            await AddEntriesToModel(model);
            var jsonModel = FileService.SerializeModelToJson(model);
            File.WriteAllText(@"C:\gw2\session\model.json", jsonModel);
        }

        private static async Task AddEntriesToModel(Model model)
        {
            var itemStats            = await ItemService.CreateItemStats();
            var manuallyCreatedStats = ManuallyCreatedStatsService.CreateManuallyCreatedStats();
            var currencyStats        = await CurrencyService.CreateCurrencyStats();

            model.Entries.AddRange(itemStats);
            model.Entries.AddRange(manuallyCreatedStats);
            model.Entries.AddRange(currencyStats);
        }
    }
}