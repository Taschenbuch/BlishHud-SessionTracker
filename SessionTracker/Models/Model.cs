using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SessionTracker.OtherServices;

namespace SessionTracker.Models
{
    public class Model : ModelVersion
    {
        public List<Stat> Stats { get; } = new List<Stat>();
        public SessionDuration SessionDuration { get; } = new SessionDuration();
        public List<StatCategory> StatCategories { get; } = new List<StatCategory>();
        public DateTime NextResetDateTimeUtc { get; set; } = UNDEFINED_RESET_DATE_TIME;
        [JsonIgnore] public static readonly DateTime UNDEFINED_RESET_DATE_TIME = new DateTime(0L, DateTimeKind.Utc); // UTC DateTime.Min
        [JsonIgnore] public static readonly DateTime NEVER_OR_ON_MODULE_START_RESET_DATE_TIME = new DateTime(3155378975999999999L, DateTimeKind.Utc); // UTC DateTime.Max
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }
        
        public void ResetDurationAndStats()
        {
            SessionDuration.ResetDuration();

            foreach (var stat in Stats)
                stat.Value.TotalAtSessionStart = stat.Value.Total;
        }

        public Stat GetStat(string statId)
        {
            return Stats.Single(e => e.Id == statId);
        }

        public void MoveSelectedStatsToTop()
        {
            var statsSortedByVisibility = Stats.OrderByDescending(stat => stat.IsVisible).ToList();
            Stats.Clear();
            Stats.AddRange(statsSortedByVisibility);
        }
    }
}