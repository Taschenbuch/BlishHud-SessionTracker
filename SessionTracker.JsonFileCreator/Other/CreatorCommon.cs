using Gw2Sharp.WebApi;
using SessionTracker.JsonFileCreator.Constants;
using SessionTracker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SessionTracker.JsonFileCreator.Other
{
    public class CreatorCommon
    {
        public static ReadOnlyCollection<Locale> Locales { get; } = new List<Locale>
        {
            Locale.English,
            Locale.French,
            Locale.German,
            Locale.Spanish,
            Locale.Chinese,
            Locale.Korean
        }.AsReadOnly();

        public static int GetIconAssetIdFromIconUrl(string iconUrl)
        {
            return int.Parse(Path.GetFileNameWithoutExtension(iconUrl));
        }

        public static void AddStatIdsToCategory(string categoryId, List<StatCategory> categories, List<string> statIds)
        {
            categories.Single(c => c.Id == categoryId)
                      .StatIds
                      .AddRange(statIds);
        }

        public static string CreateMaterialStorageSubCategoryId(int categoryApiId)
        {
            return $"{CategoryId.SUPER_MATERIAL_STORAGE} {categoryApiId}";
        }

        // DO NOT modify this without implementing a migration for older module versions
        public static string CreateCurrencyStatId(int currencyApiId)
        {
            return $"currency{currencyApiId}";
        }

        // DO NOT modify this without implementing a migration for older module versions
        public static string CreateItemStatId(int itemApiId)
        {
            return $"item{itemApiId}";
        }
    };
}
