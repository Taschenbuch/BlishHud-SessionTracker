using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class CurrencyStatsCreator
    {
        public static async Task<List<Stat>> CreateCurrencyStats()
        {
            var currencyStats = await CreateStats();
            currencyStats = StatsCreatorCommon.SetOrder(currencyStats);
            return await AddLocalizationForCurrencyNameAndDescription(currencyStats);
        }

        private static async Task<List<Stat>> CreateStats()
        {
            var stats = new List<Stat>();
            var nonObsoleteCurrencies = await GetNonObsoleteCurrenciesFromApi(Locale.English);

            foreach (var currency in nonObsoleteCurrencies)
            {
                var stat = new Stat
                {
                    Id = $"currency{currency.Id}",
                    ApiId = currency.Id,
                    ApiIdType = ApiIdType.Currency,
                    IconAssetId = StatsCreatorCommon.GetIconAssetIdFromIconUrl(currency.Icon.Url.AbsoluteUri),
                    IsVisible = false,
                    Category =
                    {
                        Type = StatCategoryType.Currencies,
                        Name =
                        {
                            LocalizedTextByLocale =
                            {
                                { Locale.English, "Currencies" },
                                { Locale.French, "monnaies" },
                                { Locale.German, "Währungen" },
                                { Locale.Spanish, "divisas" },
                            }
                        },
                    },
                };

                stats.Add(stat);
            }

            return stats;
        }

        private static async Task<List<Stat>> AddLocalizationForCurrencyNameAndDescription(List<Stat> stats)
        {
            foreach (var locale in StatsCreatorCommon.Locales)
            {
                var localCurrencies = await GetNonObsoleteCurrenciesFromApi(locale);
                
                foreach (var localCurrency in localCurrencies)
                    AddNameAndDescription(localCurrency, stats, locale);
            }

            return stats;
        }

        private static void AddNameAndDescription(Currency localCurrency, List<Stat> stats, Locale locale)
        {
            var matchingStat = stats.SingleOrDefault(e => e.ApiId == localCurrency.Id);
            matchingStat.Name.SetLocalizedText(localCurrency.Name, locale);
            matchingStat.Description.SetLocalizedText(localCurrency.Description, locale);
        }

        private static async Task<List<Currency>> GetNonObsoleteCurrenciesFromApi(Locale locale)
        {
            using var client = new Gw2Client(new Connection(locale));
            var currencies = await client.WebApi.V2.Currencies.AllAsync();
            return currencies
                .Where(c => !_obsoleteCurrencyIds.Contains(c.Id))
                .ToList();
        }

        private static readonly IReadOnlyList<int> _obsoleteCurrencyIds = new List<int>
        {
            93201, // "Red Prophet Crystal", // replaced by Blue Prophet Crystal
            93103, // "Red Prophet Shard", // replaced by BLue Prophet Shard
            86094, // "Gaeting Crystal", // replaced by Magnetite Shards
            16982, // "Ascalonian Tear", // replaced by Tales Of Dungeon Devling
            17274, // "Seal of Beetletun", // replaced by Tales Of Dungeon Devling
            17273, // "Deadly Bloom", // replaced by Tales Of Dungeon Devling
            17270, // "Manifesto of the Moletariate", // replaced by Tales Of Dungeon Devling
            17275, // "Flame Legion Charr Carving", // replaced by Tales Of Dungeon Devling
            17277, // "Symbol of Koda", // replaced by Tales Of Dungeon Devling
            17276, // "Knowledge Crystal", // replaced by Tales Of Dungeon Devling
            17272, // "Shard of Zhaitan" // replaced by Tales Of Dungeon Devling
        }.AsReadOnly();
    }
}