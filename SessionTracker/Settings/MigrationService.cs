using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using SessionTracker.Models;

namespace SessionTracker.Settings
{
    public class MigrationService
    {
        // ======== CHANGE LOG ========
        // 2.0:
        // - string IconUrl -> int IconAssetId
        // - is handled automatically because currently ref model is used with updated order and isVisible property
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
        // - values of existing stats have been modified. That does NOT include statId changes! statId changes are MAJOR changes.
        // - or new stats have been added.
        // - or old stats have been removed.
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
                if(refModel.MajorVersion != 2) // no migration required for 1 -> 2
                {
                    // migration will be implemented, when major version >1 is used for the first time
                    logger.Error($"Error: ref MajorVersion > persisted MajorVersion. " +
                                  $"A migration for that is not implemented yet. That is a bug. {versionText} " +
                                  $"Please contact the module developer in blishhud discord. :(");

                    return refModel;
                }
            }

            if (refModel.MajorVersion < persistedModel.MajorVersion)
            {
                logger.Warn($"Error: ref MajorVersion < persisted MajorVersion. " +
                            $"This can happen when previously a newer module version was installed. " +
                            $"This module version will not be able to handle the new data format. " +
                            $"Because of that it will use a fresh model from the ref folder instead. " +
                            $"{settingsResetText} {versionText} :(");

                return refModel;
            }

            refModel = UpdateStatIsVisibleInRefModel(persistedModel, refModel);
            refModel = UpdateStatsOrderInRefModel(persistedModel, refModel);
            return refModel;
        }
        
        private static Model UpdateStatIsVisibleInRefModel(Model persistedModel, Model refModel)
        {
            var isVisibleByStatId = new Dictionary<string, bool>();
            foreach (var stat in persistedModel.Stats)
                isVisibleByStatId[stat.Id] = stat.IsVisible;

            foreach (var refStat in refModel.Stats)
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression (because much harder to read)
                refStat.IsVisible = isVisibleByStatId.ContainsKey(refStat.Id)
                    ? isVisibleByStatId[refStat.Id]
                    : false; // to prevent new stats to be visible after a module version update. This can mess up the user's custom stats setup 
            }

            return refModel;
        }

        private static Model UpdateStatsOrderInRefModel(Model persistedModel, Model refModel)
        {
            var persistedIds = persistedModel.Stats
                                             .Select(e => e.Id)
                                             .ToList();

            var orderedRefStats = refModel.Stats
                                          .OrderBy(d => persistedIds.IndexOf(d.Id))
                                          .ToList();

            refModel.Stats.Clear();
            refModel.Stats.AddRange(orderedRefStats);
            return refModel;
        }
    }
}