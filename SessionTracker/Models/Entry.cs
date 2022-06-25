using System.Collections.Generic;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Entry
    {
        public string Id { get; set; } = string.Empty;
        public string LabelText { get; set; } = string.Empty;
        public string LabelTooltip { get; set; } = string.Empty;
        public string PlaceholderInTooltip { get; set; } = string.Empty;
        public int AchievementId { get; set; }
        public int CurrencyId { get; set; }
        public string IconUrl { get; set; }
        public string IconFileName { get; set; }
        public bool IsVisible { get; set; } = true;
        [JsonIgnore] public Value Value { get; } = new Value();
        [JsonIgnore] public List<string> SessionHistory { get; } = new List<string>();
        [JsonIgnore] public bool IsAchievement => AchievementId != 0;
        [JsonIgnore] public bool IsCurrency => CurrencyId != 0;
        [JsonIgnore] public bool HasIcon => HasIconFile || HasIconUrl;
        [JsonIgnore] public bool HasIconFile => IconFileName != null;
        [JsonIgnore] public bool HasIconUrl => IconUrl != null;
    }
}