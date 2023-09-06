using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.Models
{
    public static class StatsExtensions
    {
        public static List<Stat> WhereUserSetToBeVisible(this IEnumerable<Stat> stats)
        {
            return stats.Where(e => e.IsVisible)
                        .ToList();
        }

        public static List<Stat> WhereSessionValueIsNonZero(this IEnumerable<Stat> stats)
        {
            return stats.Where(e => e.Value.Session != 0)
                        .ToList();
        }
    }
}