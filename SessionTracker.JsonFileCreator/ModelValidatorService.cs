using SessionTracker.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SessionTracker.JsonFileCreator
{
    public class ModelValidatorService
    {
        public static void ThrowIfModelIsInvalid(Model model)
        {
            ThrowIfStatHasNoCategory(model.Stats); // todo x testen
            ThrowIfCategoryIdsAreNotUnique(model.StatCategories); // todo x testen
        }

        private static void ThrowIfCategoryIdsAreNotUnique(List<StatCategory> statCategories)
        {
            var statCategoryIds = statCategories.Select(c => c.Id).ToList();
            var idsAreNotUnique = statCategoryIds.Count() != statCategoryIds.Distinct().Count();
            if (idsAreNotUnique)
            {
                var notUniqueIds = statCategoryIds.Except(statCategoryIds.Distinct()).ToList();
                throw new Exception($"Error: category ids must be unique. not unique category ids: {string.Join(" ,", notUniqueIds)}");
            }
        }

        private static void ThrowIfStatHasNoCategory(List<Stat> stats)
        {
            // todo x fix: prüfen dass alle stats mindestens einer category zugeordnet sind
            //    foreach (var stat in stats) // 
            //        if (string.IsNullOrWhiteSpace(stat.CategoryId))
            //            throw new Exception($"Error: CategoryId must be set. stat: {stat.Name.English} (id: {stat.Id}, apiId: {stat.ApiId})");
        }
    }
}
