using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class ModelVersion
    {
        [JsonProperty(Order = -2)] public int Version { get; set; } = 3; // Order -2: improves readability by moving it to the top of model.json.
    }
}

// ======== GENERAL ========
// - This is the model.json data format version. NOT the module version. NOT the content_version from blish static host.
// - It is connected to format_version from blish static host, but there is no 1:1 relation (e.g. format_version_1 is related to Version 2 and 3)
// - When to update Version:
// model format has changed in a not forward/backward compatible way.
// e.g. statIds of existing stats have been modified.
// But adding new stats does not require a Version update (e.g. adding another item stat when other item stats already exist).
// e.g. the model format has been changed in a way that requires a migration.
// Not all model format changes require this (e.g. Version 2 was not really necessary)
// That means an older module version cannot handle the newer model.json version. 
//
// ======== CHANGE LOG ========
// 4:
// - (module version 9.0.0) // todo x
// - new array: StatCategories
// - IconAssetId + IconFileName -> Icon: { AssetId: 123, FileName: "wvwRank.png" }
//
// 3:
// - (module version 2.0.0, format_version_1)
// - Entries -> Stats
// - MajorVersion -> Version (handled by special method)
// 
// 2:
// - (not released internal version for trying out migration logic)
// - string IconUrl -> int IconAssetId (handled automatically because currently remote model is used with updated order and isVisible property)
//
// 1:
// - (module versions before 2.0.0)
// - In module version 2.0.0 the model.json was moved from ref folder to blish static host.
// MinorVersion property was removed. MajorVersion (= 1) was renamed to Version (= 1). Migration logic was introduced.
// Module versions lower than 2.0.0 had a MajorVersion = 1, MinorVersion = 0 and not a Version 1. Because at that point migration was planned but no concept existed yet.
// But since module version 2.0.0 this will be refered to as Version 1.
