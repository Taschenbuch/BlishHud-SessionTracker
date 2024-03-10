using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class ModelVersion
    {
        [JsonProperty(Order = -2)] public int Version { get; set; } = 4; // Order -2: improves readability by moving it to the top of model.json.
    }
}
// ======== CHANGE LOG ========
// (number is model.json Version)
// 4:
// - (module version 9.0.0, format_version_4)
// - new array: StatCategories
// - IsVisible -> IsSelectedByUser
// - IconAssetId + IconFileName -> Icon: { AssetId, FileName } (ignored by migration)
//
// 3:
// - (module version 2.0.0, format_version_1)
// - Entries -> Stats
// - MajorVersion -> Version (handled by special method)
// 
// 2:
// - (not released internal version for trying out migration logic)
// - string IconUrl -> int IconAssetId (ignored by migration because because remote model only needs stats order and isVisible property from local model.json)
//
// 1:
// - (module versions before 2.0.0)
//
//
// ======== GENERAL ========
// - This is the model.json format version (short: Version). It is NOT: module version, format_version or content_version from blish static host.
// - Version is related to format_version_* from blish static host in a 1:1 relation. But their value dont have to be the same.
// e.g. format_version_1 includes only Version 3
// e.g. format_version_4 includes only Version 4
// (format_version_2 and format_version_3 were skipped to make it easier to see the relation between format_version and Version)
// - the format_version_* defines from which folder the remote model.json has to be downloaded.
// This includes updates when a new content_version is available. This never requires a migration.
// - For the migration process of the local model.json only Version is relevant. Because that is the only way to compare the local model.json with the remote model.json
// - When is a Version update or a new format_version_* folder required?
//  - A Version update is REQUIRED when the model format changes:
//   - property was renamed. e.g. Id: 1 -> ApiId: 1
//   - property type changed. e.g. Id: "wvw" -> Id: 123 -> Id: { internal: "wvw", external: 123 }
//   - property value changed that the module uses a constant (for finding it etc). e.g. Id: "wvw" -> Id: "world vs world" (applies to statIds, currencyIds, ItemIds, ...)
//  - The Version must NOT be updated:
//   - array gets new values. e.g. new stat or stat category is added
//   - property value changed that is read dynamically by the module. e.g. display texts like localized name/description.
//  - A new format_version_* folder is required every time the Version is updated. The Version of a model.json inside a format_version folder must not be changed.
//   - This ensures, that older module versions can still download a remote model.json from their format_version folder without a risk of incompatibilities.
// - Version update workflow
//  - increase Version property in module code by 1.
//  - run model.json creator to get a model.json with the new Version and the new format.
//  - create a new format_version_* folder. Place the new model.json into it. Place a content_version.txt into it with 1 as content.
//  - update the format_version constant in the module
// - partial migration of local model.json:
// An old local model.json (from an old module version after updating the module) is migrated to the new format because the module has to read some parts of local model.json.
// Those are used to update the remote model.json which then will be used by the module as the new local model.json.
// Old local model.json parts that are not read dont have to be migrated because they will not be accessed by the module.
// This will result in a partially invalid Model object created by newtonsoft json. Some properties of the c# model will contain a default value (e.g. null).
// That is because the json structure of the c# model and the only partially migrated local model.json do not match. 
// But that is not an issue because the partially migrated model.json is not used further. It will be replaced after the relevant parts were read.
// - history: switch from ref folder to static host:
// In module version 2.0.0 the model.json was moved from ref folder to blish static host.
// MinorVersion property was removed. MajorVersion (= 1) was renamed to Version (= 1). Migration logic was introduced.
// Module versions lower than 2.0.0 had a MajorVersion = 1, MinorVersion = 0 and not a Version 1. Because at that point migration was planned but not implemented yet.
// But since module version 2.0.0 this will be refered to as Version 1.
// So the first real migration that took place for released versions was from Version 1 (MajorVersion 1, MinorVersion 0) to Version 3, skipping Version 2. At
// the same time the blish static host was used for the first time. Because of that the first Version on the static host used by a released module version
// was Version 3, format_version_1, content_version 1. It would have been more appropriate to put it into format_version_3.
