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
            await AddEntriesToModel(model);
            var jsonModel = FileService.SerializeModelToJson(model);
            File.WriteAllText(@"C:\Dev\blish\model.json", jsonModel);
        }

        private static async Task AddEntriesToModel(Model model)
        {
            var itemStats     = await ItemService.CreateItemStats();
            var currencyStats = await CurrencyStatService.CreateCurrencyStats();

            model.Entries.AddRange(itemStats);
            model.Entries.AddRange(CustomStatService.CustomStats);
            model.Entries.AddRange(currencyStats);
        }
    }
}