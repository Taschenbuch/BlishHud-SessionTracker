using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    public class CurrencyService
    {
        public static async Task<List<Entry>> CreateCurrencyStats()
        {
            var entries = new List<Entry>();

            await CreateStats(entries);
            await AddLocalizedTexts(entries);

            return entries;
        }

        private static async Task CreateStats(List<Entry> entries)
        {
            using (var client = new Gw2Client(new Connection(Locale.English)))
            {
                var currencies = await client.WebApi.V2.Currencies.AllAsync();

                foreach (var currency in currencies)
                {
                    var entry = new Entry
                    {
                        Id         = $"currency{currency.Id}",
                        CurrencyId = currency.Id,
                        IconUrl    = currency.Icon.Url.ToString(),
                        IsVisible  = false
                    };

                    entries.Add(entry);
                }
            }
        }

        private static async Task AddLocalizedTexts(List<Entry> entries)
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
        }

        private static void UpdateTexts(Currency currency, List<Entry> entries, Locale local)
        {
            var entryForCurrencyExists = entries.Any(e => e.CurrencyId == currency.Id);
            if (entryForCurrencyExists)
            {
                var matchingEntry = entries.Single(e => e.CurrencyId == currency.Id);
                matchingEntry.LabelText.SetLocalizedText(currency.Name, local);
                matchingEntry.LabelTooltip.SetLocalizedText($"{currency.Name}\n{currency.Description}", local);
            }
        }
    }
}