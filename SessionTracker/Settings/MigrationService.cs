using Blish_HUD;
using Newtonsoft.Json.Linq;
using SessionTracker.Models;
using System;
using System.Collections.Generic;

// ======== CHANGE LOG ========
// 3.0:
// - Entries -> Stats
// - MajorVersion -> Version (will be handled in special method)
// 
// 2.0:
// - string IconUrl -> int IconAssetId (is handled automatically because currently ref model is used with updated order and isVisible property)
// - was never part of a release because 3.0 came earlier. 2.0 and 3.0 were still not merged into 2.0 because they were usefull to test migration logic.
//
// ======== Version definitions ======== 
// - model format has changed
// - OR statIds of existing stats have been modified.

namespace SessionTracker.Settings
{
    public static class MigrationService
    {
        public static string MigrateModelIfIsOldVersion(string modelJson, ModelVersion modelVersion, ModelVersion refModelVersion, Logger logger)
        {
            var versionSummaryText = $"persisted version: {modelVersion.Version}; ref folder version: {refModelVersion.Version}.";
            var settingsWillBeResetedText = "This may result in resetting settings changed by the user like stats visibility/order.";
            ThrowIfMigrationMethodIsMissing(refModelVersion, versionSummaryText, settingsWillBeResetedText);
            ThrowIfModelVersionIsTooNew(modelVersion, refModelVersion, logger, versionSummaryText, settingsWillBeResetedText);
            return MigrateModelJson(ref modelJson, modelVersion, refModelVersion, logger);
        }

        private static void ThrowIfMigrationMethodIsMissing(ModelVersion refModelVersion, string versionSummaryText, string settingsWillBeResetedText)
        {
            var isMigrationMethodMissing = refModelVersion.Version - 1 > _migrationMethods.Count;
            if (isMigrationMethodMissing)
                throw new MigrationException($"Migration method missing. {_migrationMethods.Count} migration methods exist. {versionSummaryText} {settingsWillBeResetedText}");
        }

        private static void ThrowIfModelVersionIsTooNew(ModelVersion modelVersion, ModelVersion refModelVersion, Logger logger, string versionSummaryText, string settingsWillBeResetedText)
        {
            if (refModelVersion.Version < modelVersion.Version)
            {
                logger.Warn($"ref Version < persisted Version. " +
                            $"This can happen when previously a newer module version was installed. " +
                            $"This module version will not be able to handle the new data format. " +
                            $"Because of that it will use a fresh model from the ref folder instead. " +
                            $"{settingsWillBeResetedText} {versionSummaryText} :(");

                throw new MigrationException($"ref Version < persisted Version. {versionSummaryText} {settingsWillBeResetedText}");
            }
        }

        // e.g. migration from 1 to 2 has to call method1to2 [0]
        // e.g. migration from 1 to 3 has to call method1to2 [0], method2to3 [1]
        // e.g. migration from 2 to 5 has to call method2to3 [1], method3to4 [2], method4to5 [3]
        // e.g. migration from 4 to 5 has to call method4to5 [3]
        private static string MigrateModelJson(ref string modelJson, ModelVersion modelVersion, ModelVersion refModelVersion, Logger logger)
        {
            for (var methodIndex = modelVersion.Version - 1; methodIndex <= refModelVersion.Version - 2; methodIndex++)
                modelJson = _migrationMethods[methodIndex](modelJson, logger);

            return modelJson;
        }

        private static List<Func<string, Logger, string>> _migrationMethods = new List<Func<string, Logger, string>>()
        {
            (modelJson, logger) => MigrateModelVersion1to2(modelJson, logger),
            (modelJson, logger) => MigrateModelVersion2To3(modelJson, logger),
        };

        // no migration required for 1 -> 2
        private static string MigrateModelVersion1to2(string modelJson, Logger logger)
        {
            logger.Info("migrate model from version 1 to 2");
            return modelJson;
        }

        private static string MigrateModelVersion2To3(string modelJson, Logger logger)
        {
            logger.Info("migrate model from version 2 to 3: rename 'Entries' to 'Stats'");
            var modelJObject = JObject.Parse(modelJson);
            modelJObject.Property("Entries").Rename("Stats");
            return modelJObject.ToString();
        }

        // dont call this with migrationMethod loop. It has to be called earlier 
        public static string RenamePropertyMajorVersionToVersion(Logger logger, string modelJson)
        {
            var modelJObject = JObject.Parse(modelJson);
            if (!modelJObject.ContainsKey(OLD_VERSION_PROPERTY_NAME))
                return modelJson;
         
            logger.Info($"migrate: rename '{OLD_VERSION_PROPERTY_NAME}' to '{NEW_VERSION_PROPERTY_NAME}'");
            modelJObject.Property(OLD_VERSION_PROPERTY_NAME).Rename(NEW_VERSION_PROPERTY_NAME);
            return modelJObject.ToString();
        }

        private const string OLD_VERSION_PROPERTY_NAME = "MajorVersion";
        private const string NEW_VERSION_PROPERTY_NAME = "Version";
    }
}