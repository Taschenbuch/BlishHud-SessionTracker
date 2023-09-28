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
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }

        public void ResetAndStartSession()
        {
            SessionDuration = new TimeSpan();
            _lastUpdateTime = DateTime.Now;

            foreach (var stat in Stats)
                stat.Value.TotalAtSessionStart = stat.Value.Total;
        }

        public void UpdateSessionDuration()
        {
            SessionDuration += DateTime.Now - _lastUpdateTime;
            _lastUpdateTime = DateTime.Now;
        }

        public Stat GetStat(string statId)
        {
            return Stats.Single(e => e.Id == statId);
        }

        private DateTime _lastUpdateTime = DateTime.Now;
    }
}