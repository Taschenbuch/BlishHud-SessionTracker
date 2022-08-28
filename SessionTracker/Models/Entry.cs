using System.Collections.Generic;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Entry
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText LabelText { get; } = new LocalizedText();
        public LocalizedText LabelTooltip { get; } = new LocalizedText();
        public int AchievementId { get; set; }
        public int CurrencyId { get; set; }
        public string IconUrl { get; set; }
        public string IconFileName { get; set; }
        public bool IsVisible { get; set; }
        [JsonIgnore] public Value Value { get; } = new Value();
        [JsonIgnore] public List<string> SessionHistory { get; } = new List<string>();
        [JsonIgnore] public bool IsAchievement => AchievementId != 0;
        [JsonIgnore] public bool IsCurrency => CurrencyId != 0;
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