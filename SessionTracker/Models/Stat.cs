﻿using System.Collections.Generic;
using Newtonsoft.Json;
using SessionTracker.Constants;

namespace SessionTracker.Models
{
    public class Stat
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText Name { get; } = new LocalizedText();
        public LocalizedText Description { get; } = new LocalizedText();
        public int ApiId { get; set; }
        public ApiIdType ApiIdType{ get; set; } = ApiIdType.None;
        public StatIcon Icon { get; } = new StatIcon();
        public bool IsSelectedByUser { get; set; }
        public Value Value { get; } = new Value();
        [JsonIgnore] public bool HasNonZeroSessionValue { get; set; } // required for coins and wvw/pvp kdr because their Value displayText is different from their SessionValue
        [JsonIgnore] public List<(string time, string sessionValueText)> SessionHistoryEntries { get; } = new List<(string time, string sessionValueText)>();
        [JsonIgnore] public bool IsAchievement => ApiIdType == ApiIdType.Achievement;
        [JsonIgnore] public bool IsCurrency => ApiIdType == ApiIdType.Currency;
        [JsonIgnore] public bool IsItem => ApiIdType == ApiIdType.Item;
        [JsonIgnore] public bool IsCoin => IsCurrency && ApiId == CurrencyId.COIN_IN_COPPER;
        [JsonIgnore] public bool IsKdr => Id == StatId.PVP_KDR || Id == StatId.WVW_KDR;

        public string GetTextWithNameAndDescription()
        {
            var descriptionExists = string.IsNullOrWhiteSpace(Description.Localized) == false;
            return descriptionExists
                ? $"{Name.Localized}\n{Description.Localized}"
                : $"{Name.Localized}";
        }
    }
}