using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.Constants
{
    public class ItemIds
    {
        public const int HEAVY_LOOT_BAG = 8920;
        public const int MEMORY_OF_BATTLE = 71581;
        public const int TRICK_OR_TREAT_BAG = 36038;
        public const int PIECE_OF_CANDY_CORN = 36041; // remove when material storage is added!
        public const int MYSTIC_COIN = 19976;  // remove when material storage is added!
        public const int FRACTAL_ENCRYPTION = 75919;

        public static readonly ReadOnlyCollection<int> Wvw = new List<int>
        {
            HEAVY_LOOT_BAG,
            MEMORY_OF_BATTLE,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Fractal = new List<int>
        {
            MYSTIC_COIN,
            FRACTAL_ENCRYPTION,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Raid = new List<int>
        {
            MYSTIC_COIN,
            FRACTAL_ENCRYPTION,
        }.AsReadOnly();
    }
}