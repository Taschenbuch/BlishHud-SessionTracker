using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.Models
{
    public static class EntriesExtensions
    {
        public static List<Entry> WhereUserSetToBeVisible(this IEnumerable<Entry> entries)
        {
            return entries.Where(e => e.IsVisible)
                          .ToList();
        }

        public static List<Entry> WhereSessionValueIsNonZero(this IEnumerable<Entry> entries)
        {
            return entries.Where(e => e.Value.Session != 0)
                          .ToList();
        }
    }
}