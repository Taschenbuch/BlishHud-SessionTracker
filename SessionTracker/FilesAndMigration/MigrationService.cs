using Newtonsoft.Json.Linq;
using SessionTracker.Files;
using SessionTracker.Models;
using SessionTracker.Other;
using System;
using System.Collections.Generic;

// =====================================================================
// For model.json CHANGE LOG see comment for model version property.
// =====================================================================
namespace SessionTracker.FilesAndMigration
{
    public static class MigrationService
    {
        // dont call this with migrationMethod loop. It has to be called earlier 
        public static string RenamePropertyMajorVersionToVersion(string modelJson)
        {
            var modelJObject = JObject.Parse(modelJson);
            if (!modelJObject.ContainsKey(OLD_VERSION_PROPERTY_NAME))
                return modelJson;

            Module.Logger.Info($"migrate: rename '{OLD_VERSION_PROPERTY_NAME}' to '{NEW_VERSION_PROPERTY_NAME}'");
            modelJObject.Property(OLD_VERSION_PROPERTY_NAME).Rename(NEW_VERSION_PROPERTY_NAME);
            return modelJObject.ToString();
        }

        public static string MigrateModelJsonIfIsOldVersion(string modelJson, ModelVersion localModelVersion, ModelVersion remoteModelVersion)
        {
            var versionSummaryText = $"persisted version: {localModelVersion.Version}; remote folder version: {remoteModelVersion.Version}.";
            var settingsWillBeResetedText = "This may result in resetting settings changed by the user like stats visibility/order.";
            ThrowIfMigrationMethodIsMissing(remoteModelVersion, versionSummaryText, settingsWillBeResetedText);
            ThrowIfModelVersionIsTooNew(localModelVersion, remoteModelVersion, versionSummaryText, settingsWillBeResetedText);
            return MigrateModelJson(modelJson, localModelVersion, remoteModelVersion);
        }

        private static void ThrowIfMigrationMethodIsMissing(ModelVersion remoteModelVersion, string versionSummaryText, string settingsWillBeResetedText)
        {
            var isMigrationMethodMissing = remoteModelVersion.Version - 1 > _migrationMethods.Count;
            if (isMigrationMethodMissing)
                throw new Exception($"Migration method missing. {_migrationMethods.Count} migration methods exist. {versionSummaryText} {settingsWillBeResetedText}");
        }

        private static void ThrowIfModelVersionIsTooNew(ModelVersion localModelVersion, ModelVersion remoteModelVersion, string versionSummaryText, string settingsWillBeResetedText)
        {
            if (remoteModelVersion.Version < localModelVersion.Version)
            {
                Module.Logger.Warn($"remote Version < local Version. " +
                                   $"This can happen when previously a newer module version was installed. " +
                                   $"This module version will not be able to handle the new data format. " +
                                   $"Because of that it will use the remote model instead. " +
                                   $"{settingsWillBeResetedText} {versionSummaryText} :(");

                throw new LogWarnException($"remote Version < local Version. {versionSummaryText} {settingsWillBeResetedText}");
            }
        }

        // e.g. migration from 1 to 2 has to call method1to2 [0]
        // e.g. migration from 1 to 3 has to call method1to2 [0], method2to3 [1]
        // e.g. migration from 2 to 5 has to call method2to3 [1], method3to4 [2], method4to5 [3]
        // e.g. migration from 4 to 5 has to call method4to5 [3]
        private static string MigrateModelJson(string modelJson, ModelVersion localModelVersion, ModelVersion remoteModelVersion)
        {
            for (var methodIndex = localModelVersion.Version - 1; methodIndex <= remoteModelVersion.Version - 2; methodIndex++)
                modelJson = _migrationMethods[methodIndex](modelJson);

            return modelJson;
        }

        private static readonly List<Func<string, string>> _migrationMethods = new List<Func<string, string>>()
        {
            (modelJson) => MigrateModelFromVersion1to2(modelJson),
            (modelJson) => MigrateModelFromVersion2To3(modelJson),
        };

        // no migration required for 1 -> 2
        private static string MigrateModelFromVersion1to2(string modelJson)
        {
            Module.Logger.Info("migrate model from version 1 to 2");
            return modelJson;
        }

        private static string MigrateModelFromVersion2To3(string modelJson)
        {
            Module.Logger.Info("migrate model from version 2 to 3: rename 'Entries' to 'Stats'");
            var modelJObject = JObject.Parse(modelJson);
            modelJObject.Property("Entries").Rename("Stats");
            return modelJObject.ToString();
        }

        private const string OLD_VERSION_PROPERTY_NAME = "MajorVersion";
        private const string NEW_VERSION_PROPERTY_NAME = "Version";
    }
}