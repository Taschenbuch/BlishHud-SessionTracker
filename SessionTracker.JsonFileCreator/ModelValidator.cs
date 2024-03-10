using SessionTracker.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SessionTracker.JsonFileCreator
{
    public static class ModelValidator
    {
        public static void ThrowIfModelIsInvalid(Model model)
        {
            ThrowIfCategoryIdsAreNotUnique(model.StatCategories);
            ThrowIfStatIdsAreNotUnique(model.Stats);
            ThrowIfSubCategoryHasNoSuperCategory(model.StatCategories);
            ThrowIfSubCategoryOfSuperCategoryDoesNotExist(model.StatCategories);
            ThrowIfStatBelongsToNoCategory(model);
            ThrowIfStatInCategoryDoesNotExist(model);
        }

        private static void ThrowIfCategoryIdsAreNotUnique(List<StatCategory> statCategories)
        {
            var statCategoryIds = statCategories.Select(c => c.Id).ToList();
            var duplicates = FindDuplicates(statCategoryIds);
            if (duplicates.Any())
                throw new Exception($"Error: category ids must be unique. not unique category ids: {string.Join(" ,", duplicates)}");
        }

        private static void ThrowIfStatIdsAreNotUnique(List<Stat> stats)
        {
            var statIds = stats.Select(c => c.Id).ToList();
            var duplicates = FindDuplicates(statIds);
            if (duplicates.Any())
                throw new Exception($"Error: stat ids must be unique. not unique stat ids: {string.Join(" ,", duplicates)}");
        }

        private static void ThrowIfSubCategoryHasNoSuperCategory(List<StatCategory> statCategories)
        {
            var subCategoryIds = statCategories
                .Where(c => c.IsSubCategory)
                .Select(c => c.Id)
                .ToList();

            var subCategoryIdsFromSuperCategories = statCategories
                .Where(c => c.IsSuperCategory)
                .SelectMany(c => c.SubCategoryIds)
                .Distinct()
                .ToList();

            var subCategoryIdsWithoutSuperCategory = subCategoryIds.Except(subCategoryIdsFromSuperCategories).ToList();
            if (subCategoryIdsWithoutSuperCategory.Any())
                throw new Exception($"Error: sub category ids which dont belong to a super category: {string.Join(" ,", subCategoryIdsWithoutSuperCategory)}");
        }

        private static void ThrowIfSubCategoryOfSuperCategoryDoesNotExist(List<StatCategory> statCategories)
        {
            var subCategoryIds = statCategories
                .Where(c => c.IsSubCategory)
                .Select(c => c.Id)
                .ToList();

            var subCategoryIdsFromSuperCategories = statCategories
                .Where(c => c.IsSuperCategory)
                .SelectMany(c => c.SubCategoryIds)
                .Distinct()
                .ToList();


            var missingSubCategoryIds = subCategoryIdsFromSuperCategories.Except(subCategoryIds).ToList();
            if (missingSubCategoryIds.Any())
                throw new Exception($"Error: sub category ids which dont exist but are part of a super category: {string.Join(" ,", missingSubCategoryIds)}");
        }

        private static void ThrowIfStatBelongsToNoCategory(Model model)
        {
            var categoryStatIds = model.StatCategories
                .SelectMany(c => c.StatIds)
                .Distinct()
                .ToList();

            var statIds = model.Stats.Select(s => s.Id).ToList();
            var statIdsWithoutCategory = statIds.Except(categoryStatIds);
            if(statIdsWithoutCategory.Any())
                throw new Exception($"Error: stat ids without category: {string.Join(" ,", statIdsWithoutCategory)}");
        }

        private static void ThrowIfStatInCategoryDoesNotExist(Model model)
        {
            var categoryStatIds = model.StatCategories
                .SelectMany(c => c.StatIds)
                .Distinct()
                .ToList();

            var statIds = model.Stats.Select(s => s.Id).ToList();
            var notExistingCategoryStatIds = categoryStatIds.Except(statIds);
            if (notExistingCategoryStatIds.Any())
                throw new Exception($"Error: category stat ids for which no stat exists: {string.Join(" ,", notExistingCategoryStatIds)}");
        }

        private static List<T> FindDuplicates<T>(List<T> values)
        {
            return values
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
        }
    }
}
