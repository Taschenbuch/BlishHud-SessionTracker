using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Model
    {
        public Version Version { get; set; } = new Version("1.0.0");
        public List<Entry> Entries { get; } = new List<Entry>();
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }

        public void StartSession()
        {
            foreach (var entry in Entries)
                entry.Value.TotalAtSessionStart = entry.Value.Total;
        }

        public Entry GetEntry(string EntryId)
        {
            return Entries.Single(e => e.Id == EntryId);
        }
    }
}