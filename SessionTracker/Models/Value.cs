using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Value
    {
        [JsonIgnore] public int Session => Total - TotalAtSessionStart;
        [JsonIgnore] public int Total { get; set; }
        public int TotalAtSessionStart { get; set; }
    }
}