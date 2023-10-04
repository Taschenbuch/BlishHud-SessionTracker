using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Model : ModelVersion
    {
        public List<Stat> Stats { get; } = new List<Stat>();
        public TimeSpan SessionDuration { get; set; }
        public DateTime NextResetDateTimeUtc { get; set; } = UNDEFINED_RESET_DATE_TIME;
        [JsonIgnore] public static readonly DateTime UNDEFINED_RESET_DATE_TIME = new DateTime(0L, DateTimeKind.Utc); // UTC DateTime.Min
        [JsonIgnore] public static readonly DateTime NEVER_OR_ON_MODULE_START_RESET_DATE_TIME = new DateTime(3155378975999999999L, DateTimeKind.Utc); // UTC DateTime.Max
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }
        
        public void ResetAndStartSession()
        {
            SessionDuration = new TimeSpan();
            _lastUpdateDateTimeUtc = DateTime.UtcNow;

            foreach (var stat in Stats)
                stat.Value.TotalAtSessionStart = stat.Value.Total;
        }

        public void UpdateSessionDuration()
        {
            SessionDuration += DateTime.UtcNow - _lastUpdateDateTimeUtc;
            _lastUpdateDateTimeUtc = DateTime.UtcNow;
        }

        public Stat GetStat(string statId)
        {
            return Stats.Single(e => e.Id == statId);
        }        

        private DateTime _lastUpdateDateTimeUtc = DateTime.UtcNow;
    }
}