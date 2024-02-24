using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Constants;
using SessionTracker.JsonFileCreator.OtherCreators;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class ItemStatsCreator
    {
        public static async Task<List<Stat>> CreateMiscItemStats()
        {
            var stats = new List<Stat>();
            using var gw2Client = new Gw2Client(new Connection(Locale.English));
            var items = await gw2Client.WebApi.V2.Items.ManyAsync(MISC_ITEM_IDS);

            foreach (var item in items)
            {
                var stat = new Stat
                {
                    Id          = $"item{item.Id}",
                    ApiId       = item.Id,
                    ApiIdType   = ApiIdType.Item,
                    Icon        =
                    {
                        AssetId = CreatorCommon.GetIconAssetIdFromIconUrl(item.Icon.Url.AbsoluteUri),
                    },
                    IsVisible   = false,
                };

                stats.Add(stat);
            }

            await AddLocalizationForItemNameAndDescription(stats);

            return stats;
        }

        public static async Task<List<Stat>> CreateMaterialStorageItemStats(List<StatCategory> categories)
        {
            var allCategoriesStats = new List<Stat>();
            using var gw2Client = new Gw2Client(new Connection(Locale.English));
            var apiMaterialCategories = await gw2Client.WebApi.V2.Materials.AllAsync();

            foreach (var apiMaterialCategory in apiMaterialCategories)
            {
                var singleCategoryStats = new List<Stat>();
                var items = await gw2Client.WebApi.V2.Items.ManyAsync(apiMaterialCategory.Items);
                foreach (var item in items)
                {
                    var statExistsAlready = allCategoriesStats.Any(s => s.ApiId == item.Id);
                    if (statExistsAlready) // because "Pile of Soybeans" (97105) exists in "Cooking Materials" and "Cooking Ingredients" Category
                        continue;

                    var stat = new Stat
                    {
                        Id = $"item{item.Id}",
                        ApiId = item.Id,
                        ApiIdType = ApiIdType.Item,
                        Icon = 
                        { 
                            AssetId = CreatorCommon.GetIconAssetIdFromIconUrl(item.Icon.Url.AbsoluteUri)
                        },
                        IsVisible = false,
                    };

                    singleCategoryStats.Add(stat);
                }

                var matchingCategory = categories.Single(c => c.Id == CreatorCommon.CreateMaterialStorageCategoryId(apiMaterialCategory.Id));
                CreatorCommon.SetPositionInCategoryAndCategoryId(singleCategoryStats, matchingCategory.Id);
                allCategoriesStats.AddRange(singleCategoryStats);
            }

            await AddLocalizationForItemNameAndDescription(allCategoriesStats);

            return allCategoriesStats;
        }

        private static async Task AddLocalizationForItemNameAndDescription(List<Stat> stats)
        {
            var itemIds = stats.Select(s => s.ApiId).ToList();

            foreach (var local in CreatorCommon.Locales)
            {
                using var gw2Client = new Gw2Client(new Connection(local));
                var localItems = await gw2Client.WebApi.V2.Items.ManyAsync(itemIds);

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