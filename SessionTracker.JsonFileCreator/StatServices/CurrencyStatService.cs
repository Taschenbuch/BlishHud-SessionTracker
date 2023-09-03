using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatServices
{
    public class CurrencyStatService
    {
        public static async Task<List<Entry>> CreateCurrencyStats()
        {
            var entries = await CreateStats();
            return await AddLocalizedTexts(entries);
        }

        private static async Task<List<Entry>> CreateStats()
        {
            var entries = new List<Entry>();

            using (var client = new Gw2Client(new Connection(Locale.English)))
            {
                var currencies = await client.WebApi.V2.Currencies.AllAsync();

                foreach (var currency in currencies)
                {
                    var entry = new Entry
                    {
                        Id          = $"currency{currency.Id}",
                        ApiId       = currency.Id,
                        ApiIdType   = ApiIdType.Currency,
                        IconAssetId = AssetService.GetIconAssetIdFromIconUrl(currency.Icon.Url.ToString()),
                        IsVisible   = false
                    };

                    entries.Add(entry);
                }
            }

            return entries;
        }

        private static async Task<List<Entry>> AddLocalizedTexts(List<Entry> entries)
        {
            var locales = new List<Locale>() { Locale.English, Locale.French, Locale.German, Locale.Spanish, Locale.Chinese, Locale.Korean };

            foreach (var local in locales)
            {
                using (var client = new Gw2Client(new Connection(local)))
                {
                    var currencies = await client.WebApi.V2.Currencies.AllAsync();

                    foreach (var currency in currencies)
                        UpdateTexts(currency, entries, local);
                }
            }

            return entries;
        }

        private static void UpdateTexts(Currency currency, List<Entry> entries, Locale local)
        {
            var entryForCurrencyExists = entries.Any(e => e.ApiId == currency.Id);
            if (entryForCurrencyExists)
            {
                var matchingEntry = entries.Single(e => e.ApiId == currency.Id);
                matchingEntry.Name.SetLocalizedText(currency.Name, local);
                matchingEntry.Description.SetLocalizedText(currency.Description, local);
            }
        }
    }
}