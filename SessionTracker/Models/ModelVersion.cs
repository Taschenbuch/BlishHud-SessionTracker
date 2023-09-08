using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class ModelVersion
    {
        [JsonProperty(Order = -2)] public int MajorVersion { get; set; } = 1; // dont use 0. 0 will probably crash migration service
    }
}
