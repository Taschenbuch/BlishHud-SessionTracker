using Gw2Sharp;
using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SessionTracker.JsonFileCreator.OtherCreators
{
    public class CategoryCreator
    {
        public static async Task<List<StatCategory>> CreateCategories()
        {
            var customCategories = CreateCustomCategories();
            var materialStorageCategories = await CreateMaterialStorageCategories();

            var categories = new List<StatCategory>();
            categories.AddRange(customCategories);
            categories.AddRange(materialStorageCategories);
            return categories;
        }

        private static async Task<List<StatCategory>> CreateMaterialStorageCategories()
        {
            using var gw2Client = new Gw2Client(new Connection(Locale.English));
            var apiMaterialCategories = await gw2Client.WebApi.V2.Materials.AllAsync();
            var categories = new List<StatCategory>();
            foreach (var apiMaterialCategory in apiMaterialCategories.OrderBy(c => c.Order).ToList())
            {
                var category = new StatCategory()
                {
                    Id = CreatorCommon.CreateMaterialStorageCategoryId(apiMaterialCategory.Id),
                };
                await SetLocalizationForMaterialStorageCategoryName(category, apiMaterialCategory.Id);
                categories.Add(category);
            }

            return categories;
        }

        private static async Task SetLocalizationForMaterialStorageCategoryName(StatCategory category, int categoryApiId)
        {
            foreach (var locale in CreatorCommon.Locales)
            {
                using var gw2Client = new Gw2Client(new Connection(locale));
                var localApiCategory = await gw2Client.WebApi.V2.Materials.GetAsync(categoryApiId);
                category.Name.SetLocalizedText(localApiCategory.Name, locale);
            }
        }

        private static List<StatCategory> CreateCustomCategories()
        {
            return new List<StatCategory>()
            {
                new StatCategory()
                {
                    Id   = CategoryId.MISC,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Misc",
                            [Locale.French]  = "Autre",
                            [Locale.German]  = "Sonstiges",
                            [Locale.Spanish] = "Otros",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.WVW,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "WvW",
                            [Locale.French]  = "McM",
                            [Locale.German]  = "WvW",
                            [Locale.Spanish] = "McM",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.PVP,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "PvP",
                            [Locale.French]  = "JvJ",
                            [Locale.German]  = "PvP",
                            [Locale.Spanish] = "PvP",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.CURRENCY,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Currencies",
                            [Locale.French]  = "monnaies",
                            [Locale.German]  = "Währungen",
                            [Locale.Spanish] = "divisas",
                        }
                    }
                },
            };
        }
    }
}
