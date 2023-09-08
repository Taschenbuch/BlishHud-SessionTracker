using SessionTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.Settings
{
    public class RefModelService
    {
        public static Model UpdateStatIsVisibleInRefModel(Model persistedModel, Model refModel)
        {
            var persistedVisibleStatIds = new List<string>();
            foreach (var persistedVisibleStat in persistedModel.Stats.Where(s => s.IsVisible))
                persistedVisibleStatIds.Add(persistedVisibleStat.Id);

            foreach (var refStat in refModel.Stats)
                refStat.IsVisible = persistedVisibleStatIds.Contains(refStat.Id); // new stats are not visible to prevent messing up users custom stats setup

            return refModel;
        }

        public static Model UpdateStatsOrderInRefModel(Model persistedModel, Model refModel)
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
