using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class ModelVersion
    {
        [JsonProperty(Order = -2)] public int Version { get; set; } = 3; // Order -2 moves it to top of model.json. todo x comment verbessern.
                                                                         // todo x aller erste version war schon 3. gab nie 1 oder 2?
                                                                         // wahrscheinlich waren 1-2 migrations testversionen. Fall ja -> migration für diese ggf. löschen?
                                                                         // VORSICHT: es muss EINE version gegeben haben BEVOR ich auf static host umgestiegen bin!
    }
}
