using Blish_HUD.Content;
using System.Collections.Generic;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltipContent
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AsyncTexture2D IconTexture { get; internal set; }
        public string SessionValuePerHourText { get; set; } = string.Empty;
        public string TotalValueText { get; set; } = string.Empty;
        public string SessionValueText { get; set; } = string.Empty;
        public string SessionDurationText { get; set; } = string.Empty;
        public List<(string time, string sessionValueText)> SessionHistoryEntries { get; set; } = new List<(string time, string sessionValueText)>();
    }
}
