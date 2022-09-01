using System.IO;
using System.Threading.Tasks;
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
            var itemStats     = await ItemService.CreateItemStats();
            var customStats   = CustomStatService.CreateCustomStats();
            var currencyStats = await CurrencyStatService.CreateCurrencyStats();

            model.Entries.AddRange(itemStats);
            model.Entries.AddRange(customStats);
            model.Entries.AddRange(currencyStats);
        }
    }
}