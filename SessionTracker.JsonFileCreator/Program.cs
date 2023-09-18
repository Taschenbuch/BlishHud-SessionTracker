using System.IO;
using System.Threading.Tasks;
using SessionTracker.JsonFileCreator.StatServices;
using SessionTracker.Models;
using SessionTracker.Settings;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        static async Task Main()
        {
            var model = new Model();
            await AddStatsToModel(model);
            var jsonModel = JsonService.SerializeModelToJson(model);
            File.WriteAllText(@"C:\Dev\blish\model.json", jsonModel);
        }

        private static async Task AddStatsToModel(Model model)
        {
            var itemStats     = await ItemService.CreateItemStats();
            var currencyStats = await CurrencyStatService.CreateCurrencyStats();

            model.Stats.AddRange(itemStats);
            model.Stats.AddRange(CustomStatService.CustomStats);
            model.Stats.AddRange(currencyStats);
        }
    }
}