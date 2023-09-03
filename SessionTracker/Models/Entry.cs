using System.Collections.Generic;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Entry
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText Name { get; } = new LocalizedText();
        public LocalizedText Description { get; } = new LocalizedText();
        public int ApiId { get; set; }
        public ApiIdType ApiIdType{ get; set; } = ApiIdType.None;
        public int IconAssetId { get; set; }
        public string IconFileName { get; set; }
        public bool IsVisible { get; set; }
        [JsonIgnore] public Value Value { get; } = new Value();
        [JsonIgnore] public List<string> SessionHistory { get; } = new List<string>();
        [JsonIgnore] public bool IsAchievement => ApiIdType == ApiIdType.Achievement;
        [JsonIgnore] public bool IsCurrency => ApiIdType == ApiIdType.Currency;
        [JsonIgnore] public bool IsItem => ApiIdType == ApiIdType.Item;
        [JsonIgnore] public bool HasIconFile => IconFileName != null;
        [JsonIgnore] public bool HasIconAssetId => IconAssetId != 0;

        public string GetTextWithNameAndDescription()
        {
            var descriptionExists = string.IsNullOrWhiteSpace(Description.Localized) == false;
            return descriptionExists
                ? $"{Name.Localized}\n{Description.Localized}"
                : $"{Name.Localized}";
        }
    }
}