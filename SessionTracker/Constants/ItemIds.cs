using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.Constants
{
    public class ItemIds
    {
        public const int HEAVY_LOOT_BAG = 8920;
        public const int MEMORY_OF_BATTLE = 71581;

        public static readonly ReadOnlyCollection<int> Wvw = new List<int>
        {
            HEAVY_LOOT_BAG,
            MEMORY_OF_BATTLE,
        }.AsReadOnly();
    }
}