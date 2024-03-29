﻿using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.Api
{
    public class AchievementTotalValueService
    {
        public static void SetAchievementTotalValues(Model model, Task<IApiV2ObjectList<AccountAchievement>> achievementsTask)
        {
            foreach (var stat in model.Stats.Where(v => v.IsAchievement)) 
                stat.Value.Total = GetAchievementValue(achievementsTask, stat.ApiId, stat.Value.Total);
        }

        private static int GetAchievementValue(Task<IApiV2ObjectList<AccountAchievement>> achievementsTask, int achievementId, int oldValue)
        {
            // reason for FirstOrDefault: for achievements that have a value of 0, the achievement is missing in the api response.
            var newValue = achievementsTask.Result.FirstOrDefault(a => a.Id == achievementId)
                                                 ?.Current ?? 0;

            return OtherTotalValueService.GetNewValueIfNotApiBug(newValue, oldValue);
        }
    }
}