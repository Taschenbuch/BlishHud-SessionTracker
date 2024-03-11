using SessionTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.FilesAndMigration
{
    public class RemoteModelService
    {
        public static Model UpdateStatIsSelectedByUserInRemoteModel(Model localModel, Model remoteModel)
        {
            var localSelectedStatIds = new List<string>();
            foreach (var localSelectedStat in localModel.Stats.Where(s => s.IsSelectedByUser))
                localSelectedStatIds.Add(localSelectedStat.Id);

            foreach (var remoteStat in remoteModel.Stats)
                remoteStat.IsSelectedByUser = localSelectedStatIds.Contains(remoteStat.Id); // new stats are not selected to prevent messing up users custom stats setup

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
