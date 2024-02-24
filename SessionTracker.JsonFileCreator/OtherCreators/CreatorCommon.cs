using Gw2Sharp.WebApi;
using SessionTracker.Constants;
using SessionTracker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SessionTracker.JsonFileCreator.OtherCreators
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

        public static void SetPositionInCategoryAndCategoryId(List<Stat> stats, string categoryId)
        {
            for (var i = 0; i < stats.Count; i++)
            {
                stats[i].PositionInsideCategory = i + 1; // Position starts at 1 to find stats where it was not set yet (Position = 0)
                stats[i].CategoryId = categoryId;
            }
        }

        public static string CreateMaterialStorageCategoryId(int categoryApiId)
        {
            return $"{CategoryId.MATERIAL_STORAGE_ID_PREFIX} {categoryApiId}";
        }
    };
}
