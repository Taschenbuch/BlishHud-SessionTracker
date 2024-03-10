using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.JsonFileCreator.Constants;
using SessionTracker.JsonFileCreator.Other;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.Stats
{
    public class CurrencyStatsCreator
    {
        public static async Task<List<Stat>> CreateCurrencyStats()
        {
            var currencyStats = await CreateStats();
            await AddLocalizationForCurrencyNameAndDescription(currencyStats);
            return currencyStats;
        }

        private static async Task<List<Stat>> CreateStats()
        {
            var stats = new List<Stat>();
            var nonObsoleteCurrencies = await GetNonObsoleteCurrenciesFromApi(Locale.English);

            foreach (var currency in nonObsoleteCurrencies)
            {
                var stat = new Stat
                {
                    Id = CreatorCommon.CreateCurrencyStatId(currency.Id),
                    ApiId = currency.Id,
                    ApiIdType = ApiIdType.Currency,
                    Icon = 
                    { 
                        AssetId = CreatorCommon.GetIconAssetIdFromIconUrl(currency.Icon.Url.AbsoluteUri)
                    },
                    IsSelectedByUser = false,
                };

                stats.Add(stat);
            }

            return stats;
        }

        private static async Task AddLocalizationForCurrencyNameAndDescription(List<Stat> stats)
        {
            foreach (var locale in CreatorCommon.Locales)
            {
                var localCurrencies = await GetNonObsoleteCurrenciesFromApi(locale);
                
                foreach (var localCurrency in localCurrencies)
                    AddNameAndDescription(localCurrency, stats, locale);
            }
        }

        private static void AddNameAndDescription(Currency localCurrency, List<Stat> stats, Locale locale)
        {
            var matchingStat = stats.SingleOrDefault(e => e.ApiId == localCurrency.Id);
            matchingStat.Name.SetLocalizedText(localCurrency.Name, locale);
            matchingStat.Description.SetLocalizedText(localCurrency.Description, locale);
        }

        private static async Task<List<Currency>> GetNonObsoleteCurrenciesFromApi(Locale locale)
        {
            using var gw2Client = new Gw2Client(new Connection(locale));
            var currencies = await gw2Client.WebApi.V2.Currencies.AllAsync();
            return currencies
                .Where(c => !CurrencyIds.ObsoleteIds.Contains(c.Id))
                .ToList();
        }
    }
}