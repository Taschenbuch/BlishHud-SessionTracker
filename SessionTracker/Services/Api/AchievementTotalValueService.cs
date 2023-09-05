using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.Services.Api
{
    public class AchievementTotalValueService
    {
        public static void SetAchievementTotalValues(Model model, Task<IApiV2ObjectList<AccountAchievement>> achievementsTask)
        {
            foreach (var entry in model.Entries.Where(v => v.IsAchievement)) 
                entry.Value.Total = GetAchievementValue(achievementsTask, entry.ApiId, entry.Value.Total);
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