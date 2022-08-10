using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using SessionTracker.Models;

namespace SessionTracker.Settings
{
    public class MigrationService
    {
        // Version definitions
        //
        // ==== MAJOR ====
        // - model format has changed
        // - OR entryIds of existing entries have been modified.
        // - A migration has to be implemented which depends on the major versions involved.
        // - migration has to be 1 major version steps.
        // e.g. for a migration from 1.x.x. to 3.x.x the steps would be to migrate from 1.x.x to 2.x.x. and then from 2.x.x to 3.x.x
        //
        // ==== MINOR ====
        // - values of existing entries have been modified. That does NOT include entryId changes! entryId changes are MAJOR changes.
        // - or new entries have been added.
        // - or old entries have been removed.
        // - A migration has to be implemented which does not depend on the major version.
        // - INFO: no minor version handling yet. probably never. Instead the minor version handling happens every time, when there is no major version change
        // that is dirty but much easier to work with
        //
        // ==== BUILD ====
        // currently not used
        public static Model MigratePersistedModelIfNecessary(Model persistedModel, Model refModel, Logger logger)
        {
            var versionText       = $"persisted version: {persistedModel.Version}; ref folder version: {refModel.Version}.";
            var settingsResetText = "This may result in resetting settings changed by the user like stats visibility/order.";

            if (refModel.MajorVersion > persistedModel.MajorVersion)
            {
                // migration will be implemented, when major version >1 is used for the first time
                logger.Error($"Error: ref MajorVersion > persisted MajorVersion. " +
                              $"A migration for that is not implemented yet. That is a bug. {versionText} " +
                              $"Please contact the module developer in blishhud discord. :(");

                return refModel;
            }

            if (refModel.MajorVersion < persistedModel.MajorVersion)
            {
                logger.Error($"Error: ref MajorVersion <  persisted MajorVersion. " +
                              $"This can happen when previously a newer module version was installed. " +
                              $"This module version will not be able to handle the new data format. " +
                              $"Because of that it will use a fresh model from the ref folder instead. " +
                              $"{settingsResetText} {versionText} :(");

                return refModel;
            }

            refModel = UpdatedEntryIsVisibleInRefModel(persistedModel, refModel);
            refModel = UpdateEntryOrderInRefModel(persistedModel, refModel);
            return refModel;
        }
        
        private static Model UpdatedEntryIsVisibleInRefModel(Model persistedModel, Model refModel)
        {
            var isVisibleByEntryId = new Dictionary<string, bool>();
            foreach (var entry in persistedModel.Entries)
                isVisibleByEntryId[entry.Id] = entry.IsVisible;

            foreach (var refEntry in refModel.Entries)
            {
                refEntry.IsVisible = false; // to prevent new entries to be visible after a module version update. This can mess up the user's custom stats setup 

                if (isVisibleByEntryId.ContainsKey(refEntry.Id))
                    refEntry.IsVisible = isVisibleByEntryId[refEntry.Id];
            }
                

            return refModel;
        }

        private static Model UpdateEntryOrderInRefModel(Model persistedModel, Model refModel)
        {
            var persistedIds = persistedModel.Entries
                                             .Select(e => e.Id)
                                             .ToList();

            var orderedRefEntries = refModel.Entries
                                            .OrderBy(d => persistedIds.IndexOf(d.Id))
                                            .ToList();

            refModel.Entries.Clear();
            refModel.Entries.AddRange(orderedRefEntries);
            return refModel;
        }
    }
}