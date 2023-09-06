using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Model
    {
        public int MajorVersion { get; set; } = 1;
        public int MinorVersion { get; set; } = 0;
        public List<Stat> Stats { get; } = new List<Stat>();
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; } // i hate it... but using an event would suck too
        [JsonIgnore] public string Version => $"{MajorVersion}.{MinorVersion}";
        [JsonIgnore] public TimeSpan SessionDuration => DateTime.Now - _sessionStartTime; 

        public void StartSession()
        {
            _sessionStartTime = DateTime.Now;

            foreach (var stat in Stats)
                stat.Value.TotalAtSessionStart = stat.Value.Total;
        }

        public Stat GetStat(string statId)
        {
            return Stats.Single(e => e.Id == statId);
        }

        private DateTime _sessionStartTime; 
    }
}