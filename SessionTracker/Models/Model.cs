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
        public List<Entry> Entries { get; } = new List<Entry>();
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }
        [JsonIgnore] public string Version => $"{MajorVersion}.{MinorVersion}";
        [JsonIgnore] public TimeSpan SessionDuration => DateTime.Now - _sessionStartTime; 

        public void StartSession()
        {
            _sessionStartTime = DateTime.Now;

            foreach (var entry in Entries)
                entry.Value.TotalAtSessionStart = entry.Value.Total;
        }

        public Entry GetEntry(string entryId)
        {
            return Entries.Single(e => e.Id == entryId);
        }

        private DateTime _sessionStartTime; 
    }
}