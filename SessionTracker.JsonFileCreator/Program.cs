using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        static async Task Main()
        {
            var currencyEntries      = await CurrencyService.CreateCurrencyStats();
            var manuallyCreatedStats = ManuallyCreatedStatsService.CreateManuallyCreatedStats();

            var model = new Model();
            model.Entries.AddRange(manuallyCreatedStats);
            model.Entries.AddRange(currencyEntries);

            var json = JsonConvert.SerializeObject(model, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(@"C:\gw2\session\model.json", json);
        }
    }
}