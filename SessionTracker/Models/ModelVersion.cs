using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class ModelVersion
    {
        [JsonProperty(Order = -2)] public int Version { get; set; } = 3; // Order -2 moves it to top of model.json. todo x welcher zusammenhang zu content/format_version?
    }
}
