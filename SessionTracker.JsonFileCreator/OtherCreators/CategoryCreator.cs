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
            var materialStorageSubCategories = await CreateMaterialStorageCategories();

            var categories = new List<StatCategory>();
            categories.AddRange(customCategories);
            categories.AddRange(materialStorageSubCategories);
            return categories;
        }

        private static async Task<List<StatCategory>> CreateMaterialStorageCategories()
        {
            using var gw2Client = new Gw2Client(new Connection(Locale.English));
            var apiMaterialCategories = await gw2Client.WebApi.V2.Materials.AllAsync();
            var orderedApiMaterialCategories = apiMaterialCategories.OrderBy(c => c.Order).ToList();
            var categories = new List<StatCategory>
            {
                new StatCategory() // this is the super category. The other material storage categories are sub categories of this one.
                {
                    Id = CategoryId.SUPER_MATERIAL_STORAGE,
                    SubCategoryIds = orderedApiMaterialCategories.Select(c => CreatorCommon.CreateMaterialStorageSubCategoryId(c.Id)).ToList(),
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Material Storage",
                            [Locale.French]  = "Collection de matériaux",
                            [Locale.German]  = "Materialienlager",
                            [Locale.Spanish] = "Almacenamiento de material",
                        }
                    }
                }
            };

            foreach (var apiMaterialCategory in orderedApiMaterialCategories)
            {
                var subCategory = new StatCategory()
                {
                    Id = CreatorCommon.CreateMaterialStorageSubCategoryId(apiMaterialCategory.Id),
                };
                await SetLocalizationForMaterialStorageCategoryName(subCategory, apiMaterialCategory.Id);
                categories.Add(subCategory);
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
                    Id   = CategoryId.SUPER_GENERAL,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "General",
                            [Locale.French]  = "Général",
                            [Locale.German]  = "Allgemein",
                            [Locale.Spanish] = "General",
                        }
                    },
                    SubCategoryIds =
                    {
                        CategoryId.MISC,
                        CategoryId.CURRENCY,
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.SUPER_COMPETITIVE,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Competitive",
                            [Locale.French]  = "Compétitif",
                            [Locale.German]  = "Kompetitiv",
                            [Locale.Spanish] = "Competitivas",
                        }
                    },
                    SubCategoryIds =
                    {
                        CategoryId.WVW,
                        CategoryId.PVP,
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.SUPER_PVE,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "PvE",
                            [Locale.French]  = "PvE",
                            [Locale.German]  = "PvE",
                            [Locale.Spanish] = "PvE",
                        }
                    },
                    SubCategoryIds =
                    {
                        CategoryId.OPEN_WORLD,
                        CategoryId.FRACTAL,
                        CategoryId.RAID,
                        CategoryId.STRIKE,
                    }
                },
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
                    Id   = CategoryId.OPEN_WORLD,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Open World",
                            [Locale.French]  = "Monde ouvert",
                            [Locale.German]  = "Offene Welt",
                            [Locale.Spanish] = "Mundo abierto",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.RAID,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Raid",
                            [Locale.French]  = "Raid",
                            [Locale.German]  = "Schlachtzug",
                            [Locale.Spanish] = "Incursión",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.STRIKE,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Strike mission",
                            [Locale.French]  = "Mission d'attaque",
                            [Locale.German]  = "Angriffsmissionen",
                            [Locale.Spanish] = "Misiones de ataque",
                        }
                    }
                },
                new StatCategory()
                {
                    Id   = CategoryId.FRACTAL,
                    Name =
                    {
                        LocalizedTextByLocale =
                        {
                            [Locale.English] = "Fractal",
                            [Locale.French]  = "Fractale",
                            [Locale.German]  = "Fraktal",
                            [Locale.Spanish] = "Fractal",
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
                            [Locale.French]  = "Monnaies",
                            [Locale.German]  = "Währungen",
                            [Locale.Spanish] = "Divisas",
                        }
                    }
                },
            };
        }
    }
}
