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

            foreach (var currency in currencies)
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
    }
}