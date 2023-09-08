using Blish_HUD;
using Newtonsoft.Json.Linq;
using SessionTracker.Models;
using System;
using System.Collections.Generic;

// ======== CHANGE LOG ========
// 3.0:
// - Entries -> Stats
//
// 2.0:
// - string IconUrl -> int IconAssetId
// - is handled automatically because currently ref model is used with updated order and isVisible property
// - was never part of a release because 3.0 came earlier. 2.0 and 3.0 were still not merged into 2.0 because they were usefull to test migration logic.
//
// ======== Version definitions ======== 
// ==== MAJOR ====
// - model format has changed
// - OR statIds of existing stats have been modified.
// - A migration has to be implemented which depends on the major versions involved.
// - migration has to be 1 major version steps.
// e.g. for a migration from 1.x.x. to 3.x.x the steps would be to migrate from 1.x.x to 2.x.x. and then from 2.x.x to 3.x.x
//
// ==== MINOR ====
// - currently not used
// - values of existing stats have been modified. That does NOT include statId changes! statId changes are MAJOR changes.
// - or new stats have been added.
// - or old stats have been removed.
// - A migration has to be implemented which does not depend on the major version.
// - INFO: no minor version handling yet. probably never. Instead the minor version handling happens every time, when there is no major version change
// that is dirty but much easier to work with
//
// ==== BUILD ====
// currently not used

namespace SessionTracker.Settings
{
    public static class MigrationService
    {
        public static string MigrateModelIfIsOldVersion(string modelJson, ModelVersion modelVersion, ModelVersion refModelVersion, Logger logger)
        {
            var versionSummaryText = $"persisted version: {modelVersion.MajorVersion}; ref folder version: {refModelVersion.MajorVersion}.";
            var settingsWillBeResetedText = "This may result in resetting settings changed by the user like stats visibility/order.";
            throwIfMigrationMethodIsMissing(refModelVersion, versionSummaryText, settingsWillBeResetedText);
            var migrationRequired = refModelVersion.MajorVersion > modelVersion.MajorVersion;
            var noMigrationRequired = refModelVersion.MajorVersion == modelVersion.MajorVersion;
            
            if (noMigrationRequired)
                return modelJson;
            else if (migrationRequired)
                return migrateModel(modelJson, modelVersion, refModelVersion, logger);
            else // refModelVersion.MajorVersion < modelVersion.MajorVersion
            {
                logger.Warn($"ref MajorVersion < persisted MajorVersion. " +
                            $"This can happen when previously a newer module version was installed. " +
                            $"This module version will not be able to handle the new data format. " +
                            $"Because of that it will use a fresh model from the ref folder instead. " +
                            $"{settingsWillBeResetedText} {versionSummaryText} :(");

                throw new Exception($"ref MajorVersion < persisted MajorVersion. {versionSummaryText} {settingsWillBeResetedText}");
            }
        }

        private static void throwIfMigrationMethodIsMissing(ModelVersion refModelVersion, string versionSummaryText, string settingsWillBeResetedText)
        {
            var isMigrationMethodMissing = refModelVersion.MajorVersion - 1 > _migrationMethods.Count;
            if (isMigrationMethodMissing) // called even when no migration is required to prevent it from being released at all.
                throw new Exception($"Migration method missing. {_migrationMethods.Count} migration methods. {versionSummaryText} {settingsWillBeResetedText}");
        }

        // e.g. migration from 1 to 2 has to call method1to2 [0]
        // e.g. migration from 1 to 3 has to call method1to2 [0], method2to3 [1]
        // e.g. migration from 2 to 5 has to call method2to3 [1], method3to4 [2], method4to5 [3]
        // e.g. migration from 4 to 5 has to call method4to5 [3]
        private static string migrateModel(string modelJson, ModelVersion modelVersion, ModelVersion refModelVersion, Logger logger)
        {
            for (var methodIndex = modelVersion.MajorVersion - 1; methodIndex <= refModelVersion.MajorVersion - 2; methodIndex++)
                modelJson = _migrationMethods[methodIndex](modelJson, logger);

            return modelJson;
        }

        private static List<Func<string, Logger, string>> _migrationMethods = new List<Func<string, Logger, string>>()
        {
            (modelJson, logger) => migrateModelVersion1to2(modelJson, logger),
            (modelJson, logger) => migrateModelVersion2To3(modelJson, logger),
        };

        // no migration required for 1 -> 2
        private static string migrateModelVersion1to2(string modelJson, Logger logger)
        {
            logger.Info("migrate model from version 1 to 2");
            return modelJson;
        }

        private static string migrateModelVersion2To3(string modelJson, Logger logger)
        {
            logger.Info("migrate model from version 2 to 3");
            var modelJObject = JObject.Parse(modelJson);
            modelJObject.Property("Entries").Rename("Stats");
            return modelJObject.ToString();
        }
    }
}