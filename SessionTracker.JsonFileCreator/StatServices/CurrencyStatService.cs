using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatServices
{
    public class CurrencyStatService
    {
        public static async Task<List<Stat>> CreateCurrencyStats()
        {
            var stats = await CreateStats();
            return await AddLocalizedTexts(stats);
        }

        private static async Task<List<Stat>> CreateStats()
        {
            var stats = new List<Stat>();
            using var client = new Gw2Client(new Connection(Locale.English));
            var currencies = await client.WebApi.V2.Currencies.AllAsync();
            var nonObsoleteCurrencies = RemoveObsoleteCurrencies(currencies);

            foreach (var currency in nonObsoleteCurrencies)
            {
                var stat = new Stat
                {
                    Id          = $"currency{currency.Id}",
                    ApiId       = currency.Id,
                    ApiIdType   = ApiIdType.Currency,
                    IconAssetId = AssetService.GetIconAssetIdFromIconUrl(currency.Icon.Url.ToString()),
                    IsVisible   = false
                };

                stats.Add(stat);
            }

            return stats;
        }

        private static IEnumerable<Currency> RemoveObsoleteCurrencies(IApiV2ObjectList<Currency> currencies)
        {
            return currencies.Where(c => !_obsoleteCurrencyIds.Contains(c.Name));
        }

        private static async Task<List<Stat>> AddLocalizedTexts(List<Stat> stats)
        {
            var locales = new List<Locale>() { Locale.English, Locale.French, Locale.German, Locale.Spanish, Locale.Chinese, Locale.Korean };

            foreach (var local in locales)
            {
                using var client = new Gw2Client(new Connection(local));
                var currencies = await client.WebApi.V2.Currencies.AllAsync();

                foreach (var currency in currencies)
                    UpdateTexts(currency, stats, local);
            }

            return stats;
        }

        private static void UpdateTexts(Currency currency, List<Stat> stats, Locale local)
        {
            var statForCurrencyExists = stats.Any(e => e.ApiId == currency.Id);
            if (statForCurrencyExists)
            {
                var matchingStat = stats.Single(e => e.ApiId == currency.Id);
                matchingStat.Name.SetLocalizedText(currency.Name, local);
                matchingStat.Description.SetLocalizedText(currency.Description, local);
            }
        }

        private static List<string> _obsoleteCurrencyIds = new List<string>
        {
            "Red Prophet Crystal", // replaced by Blue Prophet Crystal
            "Red Prophet Shard", // replaced by BLue Prophet Shard
            "Gaeting Crystal", // replaced by Magnetite Shards
            "Ascalonian Tear", // replaced by Tales Of Dungeon Devling
            "Seal of Beetletun", // replaced by Tales Of Dungeon Devling
            "Deadly Bloom", // replaced by Tales Of Dungeon Devling
            "Manifesto of the Moletariate", // replaced by Tales Of Dungeon Devling
            "Flame Legion Charr Carving", // replaced by Tales Of Dungeon Devling
            "Symbol of Koda", // replaced by Tales Of Dungeon Devling
            "Knowledge Crystal", // replaced by Tales Of Dungeon Devling
            "Shard of Zhaitan" // replaced by Tales Of Dungeon Devling
        };
    }
}