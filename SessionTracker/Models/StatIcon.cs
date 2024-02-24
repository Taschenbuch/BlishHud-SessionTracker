using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class StatIcon
    {
        public int AssetId { get; set; }
        public string FileName { get; set; }
        [JsonIgnore] public bool HasIconFile => FileName != null;
        [JsonIgnore] public bool HasIconAssetId => AssetId != 0;
    }
}
