using System.IO;

namespace SessionTracker.JsonFileCreator.StatServices
{
    public class AssetService
    {
        public static int GetIconAssetIdFromIconUrl(string iconUrl)
        {
            return int.Parse(Path.GetFileNameWithoutExtension(iconUrl));
        }
    }
}
