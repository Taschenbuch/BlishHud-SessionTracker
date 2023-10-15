using Gw2Sharp.WebApi;
using SessionTracker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SessionTracker.JsonFileCreator.StatCreators
{
    public class StatsCreatorCommon
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

        public static List<Stat> SetOrder(List<Stat> stats)
        {
            for (var i = 0; i < stats.Count; i++)
                stats[i].Position = i + 1;

            return stats;
        }
    };
}
