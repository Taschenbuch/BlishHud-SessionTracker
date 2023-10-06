using SessionTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.Settings
{
    public class RemoteModelService
    {
        public static Model UpdateStatIsVisibleInRemoteModel(Model localModel, Model remoteModel)
        {
            var localVisibleStatIds = new List<string>();
            foreach (var localVisibleStat in localModel.Stats.Where(s => s.IsVisible))
                localVisibleStatIds.Add(localVisibleStat.Id);

            foreach (var remoteStat in remoteModel.Stats)
                remoteStat.IsVisible = localVisibleStatIds.Contains(remoteStat.Id); // new stats are not visible to prevent messing up users custom stats setup

            return remoteModel;
        }

        public static Model UpdateStatsOrderInRemoteModel(Model localModel, Model remoteModel)
        {
            var persistedIds = localModel.Stats
                                         .Select(e => e.Id)
                                         .ToList();

            var orderedRefStats = remoteModel.Stats
                                             .OrderBy(d => persistedIds.IndexOf(d.Id))
                                             .ToList();

            remoteModel.Stats.Clear();
            remoteModel.Stats.AddRange(orderedRefStats);
            return remoteModel;
        }

        public static Model UpdateTotalAtSessionStartInRemoteModel(Model localModel, Model remoteModel)
        {
            foreach (var remoteStat in remoteModel.Stats)
            {
                var localStat = localModel.Stats.Find(e => e.Id == remoteStat.Id);
                if(localStat != null)
                    remoteStat.Value.TotalAtSessionStart = localStat.Value.TotalAtSessionStart;
            }

            return remoteModel;
        }

        public static Model UpdateSessionDurationAndResetTimeInRemoteModel(Model localModel, Model remoteModel)
        {
            remoteModel.NextResetDateTimeUtc   = localModel.NextResetDateTimeUtc;
            remoteModel.SessionDuration.Value = localModel.SessionDuration.Value;
            return remoteModel;
        }
    }
}
