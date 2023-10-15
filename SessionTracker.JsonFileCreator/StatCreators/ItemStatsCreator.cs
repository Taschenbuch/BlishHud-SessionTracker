using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class ItemStatsCreator
    {
        public static async Task<List<Stat>> CreateItemStats()
        {
            var miscItemStats = await CreateMiscItemStats();
            var materialStorageItemStats = await CreateMaterialStorageItemStats();
            await AddLocalizationForMaterialStorageCategoryNames(materialStorageItemStats);

            var itemStats = new List<Stat>();
            itemStats.AddRange(miscItemStats);
            itemStats.AddRange(materialStorageItemStats);
            itemStats = StatsCreatorCommon.SetOrder(itemStats);
            await AddLocalizationForItemNameAndDescription(itemStats);
            
            return itemStats;
        }

        private static async Task<List<Stat>> CreateMiscItemStats()
        {
            var stats = new List<Stat>();
            using var client = new Gw2Client(new Connection(Locale.English));
            var items = await client.WebApi.V2.Items.ManyAsync(MISC_ITEM_IDS);

            foreach (var item in items)
            {
                var stat = new Stat
                {
                    Id          = $"item{item.Id}",
                    ApiId       = item.Id,
                    ApiIdType   = ApiIdType.Item,
                    IconAssetId = StatsCreatorCommon.GetIconAssetIdFromIconUrl(item.Icon.Url.AbsoluteUri),
                    IsVisible   = false,
                    Category =
                    {
                        Type = StatCategoryType.MiscItem,
                        Name =
                        {
                            LocalizedTextByLocale =
                            {
                                { Locale.English, "Custom Items" }, // todo x wie category nennen? oder müssen die einzelnen items eigene categories kriegen?
                                { Locale.French, "Custom Items" },
                                { Locale.German, "Custom Items" },
                                { Locale.Spanish, "Custom Items" },
                                { Locale.Chinese, "Custom Items" },
                                { Locale.Korean, "Custom Items" },
                            }
                        },
                    },
                };

                stats.Add(stat);
            }

            return stats;
        }

        private static async Task<List<Stat>> CreateMaterialStorageItemStats()
        {
            var stats = new List<Stat>();
            using var client = new Gw2Client(new Connection(Locale.English));
            var materialCategories = await client.WebApi.V2.Materials.AllAsync();
            foreach (var materialCategory in materialCategories)
            {
                var items = await client.WebApi.V2.Items.ManyAsync(materialCategory.Items);
                foreach (var item in items)
                {
                    var statExistsAlready = stats.Any(s => s.ApiId == item.Id);
                    if (statExistsAlready) // because "Pile of Soybeans" (97105) exists in "Cooking Materials" and "Cooking Ingredients" Category
                        continue;

                    var stat = new Stat
                    {
                        Id = $"item{item.Id}",
                        ApiId = item.Id,
                        ApiIdType = ApiIdType.Item,
                        IconAssetId = StatsCreatorCommon.GetIconAssetIdFromIconUrl(item.Icon.Url.AbsoluteUri),
                        IsVisible = false,
                        Category =
                        {
                            Type     = StatCategoryType.MaterialStorage,
                            ApiId    = materialCategory.Id,
                            ApiPosition = materialCategory.Order,
                        }
                    };

                    stats.Add(stat);
                }
            }
            
            return stats;
        }

        private static async Task AddLocalizationForMaterialStorageCategoryNames(List<Stat> stats)
        {
            foreach (var locale in StatsCreatorCommon.Locales)
            {
                using var client = new Gw2Client(new Connection(locale));
                var localMaterialCategories = await client.WebApi.V2.Materials.AllAsync();

                foreach (var localMaterialCategory in localMaterialCategories)
                    foreach (var stat in stats.Where(s => s.Category.ApiId == localMaterialCategory.Id))
                        stat.Category.Name.SetLocalizedText(localMaterialCategory.Name, locale);
            }
        }

        private static async Task AddLocalizationForItemNameAndDescription(List<Stat> stats)
        {
            var itemIds = stats.Select(s => s.ApiId).ToList();

            foreach (var local in StatsCreatorCommon.Locales)
            {
                using var client = new Gw2Client(new Connection(local));
                var localItems = await client.WebApi.V2.Items.ManyAsync(itemIds);

                foreach (var localItem in localItems)
                    AddNameAndDescription(localItem, stats, local);
            }
        }

        private static void AddNameAndDescription(Item localItem, List<Stat> stats, Locale locale)
        {
            var matchingStat = stats.Single(e => e.ApiId == localItem.Id);
            matchingStat.Name.SetLocalizedText(localItem.Name, locale);
            matchingStat.Description.SetLocalizedText(localItem.Description, locale);
        }

        public static readonly List<int> MISC_ITEM_IDS = new List<int>
        {
            ItemIds.HEAVY_LOOT_BAG,
            ItemIds.TRICK_OR_TREAT_BAG,
            ItemIds.FRACTAL_ENCRYPTION,
        };
    }
}