using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class Model : ModelVersion
    {
        public List<Stat> Stats { get; } = new List<Stat>();
        public SessionDuration SessionDuration { get; } = new SessionDuration();
        public List<StatCategory> StatCategories { get; } = new List<StatCategory>();
        public DateTime NextResetDateTimeUtc { get; set; } = UNDEFINED_RESET_DATE_TIME;
        [JsonIgnore] public static readonly DateTime UNDEFINED_RESET_DATE_TIME = new DateTime(0L, DateTimeKind.Utc); // UTC DateTime.Min
        [JsonIgnore] public static readonly DateTime NEVER_OR_ON_MODULE_START_RESET_DATE_TIME = new DateTime(3155378975999999999L, DateTimeKind.Utc); // UTC DateTime.Max
        [JsonIgnore] public bool UiHasToBeUpdated { get; set; }
        
        public void ResetDurationAndStats()
        {
            SessionDuration.ResetDuration();

            foreach (var stat in Stats)
                stat.Value.TotalAtSessionStart = stat.Value.Total;
        }

        public Stat GetStat(string statId)
        {
            return Stats.Single(e => e.Id == statId);
        }

        public void MoveSelectedStatsToTop()
        {
            var statsSortedByVisibility = Stats.OrderByDescending(stat => stat.IsVisible).ToList();
            Stats.Clear();
            Stats.AddRange(statsSortedByVisibility);
        }

        public List<Stat> GetDistinctStatsSortedByCategory()
        {
            return StatCategories
                .Where(c => c.IsSuperCategory)
                .SelectMany(c => c.SubCategoryIds)
                .Select(id => StatCategories.Single(c => c.Id == id))
                .SelectMany(c => c.StatIds)
                .Distinct() // get rid of stats that are in multiple categories
                .Select(id => Stats.Single(s => s.Id == id))
                .ToList();
        }

        public List<string> GetDistinctStatIds(StatCategory category)
        {
            if (category.IsSubCategory)
                return category.StatIds;

            // is SuperCategory
            return category.SubCategoryIds
                .Select(categoryId => StatCategories.Single(c => c.Id == categoryId))
                .SelectMany(category => category.StatIds)
                .Distinct() // prevents statId duplicates from stats which belong to multiple categories
                .ToList();
        }
    }
}