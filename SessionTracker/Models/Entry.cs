using System.Collections.Generic;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Entry
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText LabelText { get; } = new LocalizedText();
        public LocalizedText LabelTooltip { get; } = new LocalizedText();
        public int ApiId { get; set; }
        public ApiIdType ApiIdType{ get; set; } = ApiIdType.None;
        public string IconUrl { get; set; }
        public string IconFileName { get; set; }
        public bool IsVisible { get; set; }
        [JsonIgnore] public Value Value { get; } = new Value();
        [JsonIgnore] public List<string> SessionHistory { get; } = new List<string>();
        [JsonIgnore] public bool IsAchievement => ApiIdType == ApiIdType.Achievement;
        [JsonIgnore] public bool IsCurrency => ApiIdType == ApiIdType.Currency;
        [JsonIgnore] public bool IsItem => ApiIdType == ApiIdType.Item;
        [JsonIgnore] public bool HasIconFile => IconFileName != null;
        [JsonIgnore] public bool HasIconUrl => IconUrl != null;

        public string GetNameAndDescription()
        {
            var descriptionExists = string.IsNullOrWhiteSpace(LabelTooltip.Localized) == false;
            return descriptionExists
                ? $"{LabelText.Localized}\n{LabelTooltip.Localized}"
                : $"{LabelText.Localized}";
        }
    }
}